using BLL.CustomException;
using BLL.DTOs.UserDTO;
using BLL.Extensions;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserResponseDTO> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if(user == null)
        {
            throw new NotFoundException($"User with id {id} not found");
        }
        return user.ToUserResponseDTO();
    }
}
