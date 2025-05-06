using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using BLL.CustomException;
using BLL.DTOs.AuthenticateDTO;
using BLL.Services.Interfaces;
using Common.Utilities;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services.Implementations;

public class AuthenticateService : IAuthenticateService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    public AuthenticateService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }
    public async Task<TokenResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
    {
        var user = await _userRepository.GetUserByUsernameAsync(loginRequest.Username);
        if (user == null)
        {
            throw new NotFoundException($"User with username {loginRequest.Username} not found.");
        }
        if (!SecurityHelper.VerifyPassword(loginRequest.Password,user.PasswordHash,user.PasswordSalt))
        {
            throw new BadRequestException("Invalid password.");
        }
        // Create user claims
        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()), // Add user ID as a claim
            // Add additional claims as needed (e.g., roles, etc.)
        };
        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); // 7 days validity
        await _userRepository.SaveChangesAsync();
        var tokenHandler = new JwtSecurityTokenHandler();
        var loginResponse = new TokenResponseDTO
        {
            AccessToken = tokenHandler.WriteToken(accessToken), // Convert to stringtoken,
            RefreshToken = refreshToken,
        };
        return loginResponse;
    }
    public async Task RegisterAsync(RegisterRequestDTO registerRequest)
    {
        var existingUser = await _userRepository.GetUserByUsernameAsync(registerRequest.Username);
        if (existingUser != null)
        {
            throw new ConflictException($"User with username {registerRequest.Username} already exists.");
        }
        var (hash, salt) = SecurityHelper.HashPassword(registerRequest.Password);
        var user = new User
        {
            Username = registerRequest.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }
    public async Task<TokenResponseDTO> RetrieveAccessToken(RefreshRequestDTO refreshTokenRequest)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);
        var id = principal?.FindFirst("id")?.Value; //this is mapped to the Name claim by default
        var user = await _userRepository.GetByIdAsync(Guid.Parse(id));
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }
        if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }
        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 days validity
        await _userRepository.SaveChangesAsync();
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenResponse = new TokenResponseDTO
        {
            AccessToken = tokenHandler.WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
        };
        return tokenResponse;
    }
    public async Task LogoutAsync(string accessToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        var id = principal?.FindFirst("id")?.Value; //this is mapped to the Name claim by default
        var user = await _userRepository.GetByIdAsync(Guid.Parse(id));
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }
        user.RefreshToken = null;
        await _userRepository.SaveChangesAsync();
    }
    //public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims)
    //{

    //    var issuer = _configuration["JwtSettings:Issuer"] ?? "localhost";
    //    var audience = _configuration["JwtSettings:Audience"] ?? "localhost";
    //    var secretKey = _configuration["JwtSettings:SecretKey"] ?? "Hungprono1caobangcittisthepasskey@123";
    //    // Create a JWT
    //    var token = new JwtSecurityToken(
    //        issuer: issuer,
    //        audience: audience,
    //        claims: claims,
    //        expires: DateTime.UtcNow.AddSeconds(30), // Token expiration time
    //        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
    //            SecurityAlgorithms.HmacSha256)
    //    );

    //    return token;
    //}
    //private string GenerateRefreshToken()
    //{
    //    var randomBytes = new byte[64];
    //    using var rng = RandomNumberGenerator.Create();
    //    rng.GetBytes(randomBytes);
    //    return Convert.ToBase64String(randomBytes);
    //}
    //public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    //{
    //    var secretKey = _configuration["JwtSettings:SecretKey"] ?? "Hungprono1caobangcittisthepasskey@123";
    //    var tokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuer = false,
    //        ValidateAudience = false,
    //        ValidateIssuerSigningKey = true,
    //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
    //        ClockSkew = TimeSpan.Zero, // Disable the default 5-minute delay
    //        RoleClaimType = ClaimTypes.Role, // 👈 Tell it "which claim represents role"
    //        ValidateLifetime = false, // We want to validate the token even if it's expired
    //    };
    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    SecurityToken securityToken;
    //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
    //    var jwtSecurityToken = securityToken as JwtSecurityToken;
    //    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
    //        throw new SecurityTokenException("Invalid token");
    //    return principal;
    //}
}
