using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.AuthenticateDTO;

public class RefreshRequestDTO
{
    [Required(ErrorMessage = "AccessToken is required")]
    public required string AccessToken { get; set; }
    [Required(ErrorMessage = "RefreshToken is required")]
    public required string RefreshToken { get; set; }
}
