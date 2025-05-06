using BLL.CustomException;
using BLL.DTOs.BookBorrowingRequestDTO;
using BLL.DTOs.GenericDTO;
using BLL.DTOs.UserDTO;
using BLL.Extensions;
using BLL.Services.Interfaces;
using DAL.Repositories.Implementations;
using DAL.Repositories.Interfaces;

namespace BLL.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IBookBorrowingRequestRepository _bookBorrowingRequestRepository;
    public UserService(IUserRepository userRepository, IBookBorrowingRequestRepository bookBorrowingRequestRepository)
    {
        _userRepository = userRepository;
        _bookBorrowingRequestRepository = bookBorrowingRequestRepository;
    }
    public async Task<UserResponseDTO> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with id {id} not found");
        }
        return user.ToUserResponseDTO();
    }
    public async Task<PaginatedList<BookBorrowingRequestResponseDTO>> GetAllBookBorrowingRequestByUser(Guid id, string? status, int pageIndex, int pageSize)
    {
        var (bookBorrowingRequests, count) = await _bookBorrowingRequestRepository.GetAllBookBorrowingRequestByUser(id, status, pageIndex, pageSize);
        var bookBorrowingRequestResponseDTOs = bookBorrowingRequests.Select(x => x.ToBookBorrowingRequestResponseDTO()).ToList();
        var paginatedBookBorrowingRequests = new PaginatedList<BookBorrowingRequestResponseDTO>(bookBorrowingRequestResponseDTOs, count, pageIndex, pageSize);

        return paginatedBookBorrowingRequests;
    }
}
