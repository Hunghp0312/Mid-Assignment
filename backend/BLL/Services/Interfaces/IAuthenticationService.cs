using BLL.DTOs.AuthenticateDTO;

namespace BLL.Services.Interfaces;

public interface IAuthenticateService 
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
    Task RegisterAsync(RegisterRequestDTO registerRequest);

}
