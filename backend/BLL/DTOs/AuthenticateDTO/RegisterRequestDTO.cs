namespace BLL.DTOs.AuthenticateDTO;

public class RegisterRequestDTO
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
