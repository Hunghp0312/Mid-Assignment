using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.AuthenticateDTO;

public class RegisterRequestDTO
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public required string LastName { get; set; }
}
