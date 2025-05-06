using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BLL.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace UnitTest.Service
{
    [TestFixture]
    public class TokenServiceTest
    {
        private Mock<IConfiguration> _mockConfiguration;
        private TokenService _tokenService;
        private Mock<IConfigurationSection> _jwtSettingsSection;

        [SetUp]
        public void SetUp()
        {
            // Setup configuration mock with JWT settings
            _mockConfiguration = new Mock<IConfiguration>();
            _jwtSettingsSection = new Mock<IConfigurationSection>();

            _mockConfiguration.Setup(c => c["JwtSettings:Issuer"]).Returns("test-issuer");
            _mockConfiguration.Setup(c => c["JwtSettings:Audience"]).Returns("test-audience");
            _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"]).Returns("Hungprono1caobangcittisthepasskeyHungprono1caobangcittisthepasskeyHungprono1caobangcittisthepasskeyHungprono1caobangcittisthepa"); // At least 32 bytes for HMACSHA256

            _tokenService = new TokenService(_mockConfiguration.Object);
        }

        [Test]
        public void GenerateAccessToken_WithValidClaims_ReturnsValidToken()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("id", Guid.NewGuid().ToString()),
                new Claim("role", "User")
            };

            // Act
            var token = _tokenService.GenerateAccessToken(claims);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(token.Issuer, Is.EqualTo("test-issuer"));
                Assert.That(token.Audiences.Contains("test-audience"), Is.True);
                Assert.That(token.ValidTo > DateTime.UtcNow);
            });

            // Verify the claims are in the token
            foreach (var claim in claims)
            {
                var tokenClaim = token.Claims.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value);
                Assert.That(tokenClaim, Is.Not.Null);
            }
        }

        [Test]
        public void GenerateAccessToken_WithEmptyClaims_ReturnsTokenWithoutCustomClaims()
        {
            // Arrange
            var claims = new List<Claim>();

            // Act
            var token = _tokenService.GenerateAccessToken(claims);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(token.Issuer, Is.EqualTo("test-issuer"));
                Assert.That(token.Audiences.Contains("test-audience"), Is.True);
            });
        }

        [Test]
        public void GenerateRefreshToken_ReturnsNonEmptyString()
        {
            // Act
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.That(refreshToken, Is.Not.Null);
            Assert.That(refreshToken.Length, Is.GreaterThan(0));

            // Verify it's a valid Base64 string
            byte[] data;
            Assert.DoesNotThrow(() => data = Convert.FromBase64String(refreshToken));
        }

        [Test]
        public void GenerateRefreshToken_GeneratesUniquTokens()
        {
            // Act
            var refreshToken1 = _tokenService.GenerateRefreshToken();
            var refreshToken2 = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.That(refreshToken1, Is.Not.EqualTo(refreshToken2));
        }

        [Test]
        public void GetPrincipalFromExpiredToken_WithValidToken_ReturnsPrincipal()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim("id", userId),
                new Claim("role", "User")
            };

            var token = _tokenService.GenerateAccessToken(claims);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            // Act
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenString);

            // Assert
            Assert.That(principal, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(principal.FindFirst("id")?.Value, Is.EqualTo(userId));
                Assert.That(principal.FindFirst(ClaimTypes.Role)?.Value, Is.EqualTo("User"));
            });
        }

        [Test]
        public void GetPrincipalFromExpiredToken_WithInvalidToken_ThrowsSecurityTokenException()
        {
            // Arrange
            var invalidToken = "invalid";

            // Act & Assert
            var ex = Assert.Throws<SecurityTokenMalformedException>(() =>
                _tokenService.GetPrincipalFromExpiredToken(invalidToken));
        }

        [Test]
        public void GetPrincipalFromExpiredToken_WithExpiredToken_StillReturnsPrincipal()
        {
            // Arrange
            // Create a token with claims
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim> { new Claim("id", userId) };

            // Create an expired token manually
            var secretKey = _mockConfiguration.Object["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "test-issuer",
                audience: "test-audience",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(-10), // Expired 10 minutes ago
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            // Act
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenString);

            // Assert
            Assert.That(principal, Is.Not.Null);
            Assert.That(principal.FindFirst("id")?.Value, Is.EqualTo(userId));
        }

        [Test]
        public void GetPrincipalFromExpiredToken_WithDifferentAlgorithm_ThrowsSecurityTokenException()
        {
            // Arrange
            // Create a token with claims but using a different algorithm
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim> { new Claim("id", userId) };

            var secretKey = "Hungprono1caobangcittisthepasskeyHungprono1caobangcittisthepasskeyHungprono1caobangcittisthepasskeyHungprono1caobangcittisthepa";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512); // Using SHA512 instead of SHA256

            var token = new JwtSecurityToken(
                issuer: "test-issuer",
                audience: "test-audience",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            // Act & Assert
            var ex = Assert.Throws<SecurityTokenException>(() =>
                _tokenService.GetPrincipalFromExpiredToken(tokenString));
            Assert.That(ex.Message, Is.EqualTo("Invalid token"));
        }
    }
}