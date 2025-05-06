using BLL.DTOs.AuthenticateDTO;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticateService _authService;
    public AuthController(IAuthenticateService authService)
    {
        _authService = authService;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        var token = await _authService.LoginAsync(loginRequest);
        return Ok(token);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
    {
        await _authService.RegisterAsync(registerRequest);
        return Ok("User registered successfully.");
    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDTO refreshTokenRequest)
    {
        var token = await _authService.RetrieveAccessToken(refreshTokenRequest);
        return Ok(token);
    }
}
