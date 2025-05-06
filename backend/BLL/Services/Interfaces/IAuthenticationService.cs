using BLL.DTOs.AuthenticateDTO;

namespace BLL.Services.Interfaces;

public interface IAuthenticateService 
{
    Task<TokenResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
    Task RegisterAsync(RegisterRequestDTO registerRequest);
    Task<TokenResponseDTO> RetrieveAccessToken(RefreshRequestDTO refreshTokenRequest);
    Task LogoutAsync(string accessToken);

}
