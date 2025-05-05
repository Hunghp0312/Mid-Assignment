using BLL.DTOs.UserDTO;
using DAL.Entity;

namespace BLL.Extensions;

public static class UserMappingExtensions
{
    public static UserResponseDTO ToUserResponseDTO(this User user)
    {
        return new UserResponseDTO
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
        };
    }
}
