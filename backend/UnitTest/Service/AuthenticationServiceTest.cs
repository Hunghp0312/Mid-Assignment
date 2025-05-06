using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.CustomException;
using BLL.DTOs.AuthenticateDTO;
using BLL.Services.Implementations;
using BLL.Services.Interfaces;
using Common.Utilities;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace UnitTest.Service
{
    [TestFixture]
    public class AuthenticateServiceTest
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ITokenService> _mockTokenService;
        private AuthenticateService _authenticateService;
        private List<User> _mockUsers;
        private Guid _testUserId;
        private Guid _existingUserId;
        private JwtSecurityToken _mockAccessToken;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();

            _testUserId = Guid.NewGuid();
            _existingUserId = Guid.NewGuid();

            // Create mock users
            _mockUsers = new List<User>
            {
                new User
                {
                    Id = _testUserId,
                    Username = "testuser",
                    PasswordHash = "b9nX8oPNh4n1VLUcMFuBXaBBfok737Q1RkbphVrQu6g=",
                    PasswordSalt = "C+At2NY4wxdnriGqqg24iQ==",
                    FirstName = "Test",
                    LastName = "User",
                    Role = UserRole.User,
                    RefreshToken = "validRefreshToken",
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                },
                new User
                {
                    Id = _existingUserId,
                    Username = "existinguser",
                    PasswordHash = "OCqOmH72mdBYjZf0amTxF3OWtbesV9Lv4Y1Typ4519w=",
                    PasswordSalt = "7gKSkgQhiq+RSmknIghQ4g==",
                    FirstName = "Existing",
                    LastName = "User",
                    Role = UserRole.User
                }
            };

            // Mock JWT token
            _mockAccessToken = new JwtSecurityToken(
                issuer: "testIssuer",
                audience: "testAudience",
                claims: new List<Claim> { new Claim("id", _testUserId.ToString()) },
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("Hungprono1caobangcittisthepasskey@123")),
                    SecurityAlgorithms.HmacSha256)
            );

            // Mock repository methods
            _mockUserRepository.Setup(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((string username) => _mockUsers.Find(u => u.Username == username));

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _mockUsers.Find(u => u.Id == id));

            _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Callback((User user) => _mockUsers.Add(user))
                .ReturnsAsync((User user) => user);

            _mockUserRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Mock token service
            _mockTokenService.Setup(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
                .Returns(_mockAccessToken);

            _mockTokenService.Setup(ts => ts.GenerateRefreshToken())
                .Returns("newRefreshToken");

            // Initialize the service
            _authenticateService = new AuthenticateService(_mockUserRepository.Object, _mockTokenService.Object);
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsTokenResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "testuser",
                Password = "user2"
            };

            // Act
            var result = await _authenticateService.LoginAsync(loginRequest);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.Not.Null.Or.Empty);
            Assert.That(result.RefreshToken, Is.Not.Null.Or.Empty);

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(loginRequest.Username), Times.Once);
            _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            // Verify token service calls
            _mockTokenService.Verify(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
            _mockTokenService.Verify(ts => ts.GenerateRefreshToken(), Times.Once);
        }

        [Test]
        public void LoginAsync_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "nonexistentuser",
                Password = "password123"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(
                async () => await _authenticateService.LoginAsync(loginRequest));
            Assert.That(ex.Message, Is.EqualTo($"User with username {loginRequest.Username} not found."));

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(loginRequest.Username), Times.Once);

            // Verify token service is not called
            _mockTokenService.Verify(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
            _mockTokenService.Verify(ts => ts.GenerateRefreshToken(), Times.Never);
        }

        [Test]
        public void LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "testuser",
                Password = "user1"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadRequestException>(
                async () => await _authenticateService.LoginAsync(loginRequest));
            Assert.That(ex.Message, Is.EqualTo("Invalid password."));

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(loginRequest.Username), Times.Once);

            // Verify token service is not called
            _mockTokenService.Verify(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
            _mockTokenService.Verify(ts => ts.GenerateRefreshToken(), Times.Never);
        }

        [Test]
        public async Task RegisterAsync_ValidData_AddsUser()
        {
            // Arrange
            var registerRequest = new RegisterRequestDTO
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User"
            };


            // Track initial count
            int initialUserCount = _mockUsers.Count;

            // Act
            await _authenticateService.RegisterAsync(registerRequest);

            // Assert
            Assert.That(_mockUsers.Count, Is.EqualTo(initialUserCount + 1));
            var newUser = _mockUsers.Find(u => u.Username == registerRequest.Username);
            Assert.That(newUser, Is.Not.Null);
            Assert.That(newUser.FirstName, Is.EqualTo(registerRequest.FirstName));
            Assert.That(newUser.LastName, Is.EqualTo(registerRequest.LastName));

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(registerRequest.Username), Times.Once);
            _mockUserRepository.Verify(repo => repo.AddAsync(It.Is<User>(u => u.Username == registerRequest.Username)), Times.Once);
            _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void RegisterAsync_ExistingUser_ThrowsConflictException()
        {
            // Arrange
            var registerRequest = new RegisterRequestDTO
            {
                Username = "existinguser", // This username already exists in our mock data
                Password = "password123",
                FirstName = "Existing",
                LastName = "User"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ConflictException>(
                async () => await _authenticateService.RegisterAsync(registerRequest));
            Assert.That(ex.Message, Is.EqualTo($"User with username {registerRequest.Username} already exists."));

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetUserByUsernameAsync(registerRequest.Username), Times.Once);
            _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
            _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task RetrieveAccessToken_ValidRefreshToken_ReturnsNewTokens()
        {
            // Arrange
            var user = _mockUsers.Find(u => u.Username == "testuser");
            var refreshRequest = new RefreshRequestDTO
            {
                AccessToken = "expiredToken",
                RefreshToken = user.RefreshToken
            };

            // Mock GetPrincipalFromExpiredToken to return valid claims
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _mockTokenService.Setup(ts => ts.GetPrincipalFromExpiredToken(refreshRequest.AccessToken))
                .Returns(claimsPrincipal);

            // Act
            var result = await _authenticateService.RetrieveAccessToken(refreshRequest);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.Not.Null.Or.Empty);
            Assert.That(result.RefreshToken, Is.Not.Null.Or.Empty);
            Assert.That(result.RefreshToken, Is.Not.EqualTo(refreshRequest.RefreshToken));

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(user.Id), Times.Once);
            _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            // Verify token service calls
            _mockTokenService.Verify(ts => ts.GetPrincipalFromExpiredToken(refreshRequest.AccessToken), Times.Once);
            _mockTokenService.Verify(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
            _mockTokenService.Verify(ts => ts.GenerateRefreshToken(), Times.Once);
        }

        [Test]
        public void RetrieveAccessToken_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var refreshRequest = new RefreshRequestDTO
            {
                AccessToken = "expiredToken",
                RefreshToken = "someRefreshToken"
            };

            // Mock GetPrincipalFromExpiredToken to return claims with non-existent user ID
            var nonExistentUserId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("id", nonExistentUserId.ToString()),
                new Claim("role", UserRole.User.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _mockTokenService.Setup(ts => ts.GetPrincipalFromExpiredToken(refreshRequest.AccessToken))
                .Returns(claimsPrincipal);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(
                async () => await _authenticateService.RetrieveAccessToken(refreshRequest));
            Assert.That(ex.Message, Is.EqualTo($"User with ID {nonExistentUserId} not found."));

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(nonExistentUserId), Times.Once);

            // Verify token service calls
            _mockTokenService.Verify(ts => ts.GetPrincipalFromExpiredToken(refreshRequest.AccessToken), Times.Once);
            _mockTokenService.Verify(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
            _mockTokenService.Verify(ts => ts.GenerateRefreshToken(), Times.Never);
        }

        [Test]
        public void RetrieveAccessToken_InvalidRefreshToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = _mockUsers.Find(u => u.Username == "testuser");
            var refreshRequest = new RefreshRequestDTO
            {
                AccessToken = "expiredToken",
                RefreshToken = "invalidRefreshToken"  // Different from user's refresh token
            };

            // Mock GetPrincipalFromExpiredToken to return valid claims
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _mockTokenService.Setup(ts => ts.GetPrincipalFromExpiredToken(refreshRequest.AccessToken))
                .Returns(claimsPrincipal);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _authenticateService.RetrieveAccessToken(refreshRequest));
            Assert.That(ex.Message, Is.EqualTo("Invalid refresh token."));

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(user.Id), Times.Once);
            _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);

            // Verify token service calls
            _mockTokenService.Verify(ts => ts.GetPrincipalFromExpiredToken(refreshRequest.AccessToken), Times.Once);
            _mockTokenService.Verify(ts => ts.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
            _mockTokenService.Verify(ts => ts.GenerateRefreshToken(), Times.Never);
        }

        [Test]
        public void RetrieveAccessToken_ExpiredRefreshToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = _mockUsers.Find(u => u.Username == "testuser");
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1); // Expired token

            var refreshRequest = new RefreshRequestDTO
            {
                AccessToken = "expiredToken",
                RefreshToken = user.RefreshToken
            };

            // Mock GetPrincipalFromExpiredToken to return valid claims
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _mockTokenService.Setup(ts => ts.GetPrincipalFromExpiredToken(refreshRequest.AccessToken))
                .Returns(claimsPrincipal);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _authenticateService.RetrieveAccessToken(refreshRequest));
            Assert.That(ex.Message, Is.EqualTo("Invalid refresh token."));

            // Reset the expiry time for other tests
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        }

        [Test]
        public async Task LogoutAsync_ValidToken_ClearsRefreshToken()
        {
            // Arrange
            var user = _mockUsers.Find(u => u.Username == "testuser");
            Assert.That(user.RefreshToken, Is.Not.Null);
            var accessToken = "validToken";

            // Mock GetPrincipalFromExpiredToken to return valid claims
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _mockTokenService.Setup(ts => ts.GetPrincipalFromExpiredToken(accessToken))
                .Returns(claimsPrincipal);

            // Act
            await _authenticateService.LogoutAsync(accessToken);

            // Assert
            Assert.That(user.RefreshToken, Is.Null);

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(user.Id), Times.Once);
            _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            // Verify token service calls
            _mockTokenService.Verify(ts => ts.GetPrincipalFromExpiredToken(accessToken), Times.Once);
        }

        [Test]
        public void LogoutAsync_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var accessToken = "validToken";

            // Mock GetPrincipalFromExpiredToken to return claims with non-existent user ID
            var nonExistentUserId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("id", nonExistentUserId.ToString()),
                new Claim("role", UserRole.User.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _mockTokenService.Setup(ts => ts.GetPrincipalFromExpiredToken(accessToken))
                .Returns(claimsPrincipal);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(
                async () => await _authenticateService.LogoutAsync(accessToken));
            Assert.That(ex.Message, Is.EqualTo($"User with ID {nonExistentUserId} not found."));

            // Verify repository calls
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(nonExistentUserId), Times.Once);
            _mockUserRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);

            // Verify token service calls
            _mockTokenService.Verify(ts => ts.GetPrincipalFromExpiredToken(accessToken), Times.Once);
        }
    }
}