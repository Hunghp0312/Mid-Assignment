using BLL.DTOs.UserDTO;

namespace BLL.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> GetUserByIdAsync(Guid id);
}
