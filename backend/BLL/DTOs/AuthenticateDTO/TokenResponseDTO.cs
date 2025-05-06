namespace BLL.DTOs.AuthenticateDTO;

public class TokenResponseDTO
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
