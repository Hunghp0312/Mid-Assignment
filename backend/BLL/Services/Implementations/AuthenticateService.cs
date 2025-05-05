using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    public AuthenticateService(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }
    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
    {
        var user = await _userRepository.GetUserByUsernameAsync(loginRequest.Username);
        if (user == null)
        {
            throw new NotFoundException($"User with username {loginRequest.Username} not found.");
        }
        if (!SecurityHelper.VerifyPassword(loginRequest.Password,user.PasswordHash,user.PasswordSalt))
        {
            throw new UnauthorizedAccessException("Invalid password.");
        }
        var issuer = _configuration["JwtSettings:Issuer"] ?? "localhost";
        var audience = _configuration["JwtSettings:Audience"] ?? "localhost";
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? "Hungprono1caobangcittisthepasskey@123";
        var token = GenerateAccessToken(user.Id,user.Role, issuer, audience, secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var loginResponse = new LoginResponseDTO
        {
            AccessToken = tokenHandler.WriteToken(token) // Convert to stringtoken,
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
    public static JwtSecurityToken GenerateAccessToken(Guid id, UserRole role, string issuer, string audience, string secretKey)
    {

        // Create user claims
        var claims = new List<Claim>
        {
            new Claim("id", id.ToString()),
            new Claim("role", role.ToString()), // Add user ID as a claim
            // Add additional claims as needed (e.g., roles, etc.)
        };

        // Create a JWT
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(10), // Token expiration time
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
