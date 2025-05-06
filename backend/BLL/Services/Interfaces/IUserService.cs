using BLL.DTOs.BookBorrowingRequestDTO;
using BLL.DTOs.GenericDTO;
using BLL.DTOs.UserDTO;

namespace BLL.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> GetUserByIdAsync(Guid id);
    Task<PaginatedList<BookBorrowingRequestResponseDTO>> GetAllBookBorrowingRequestByUser(Guid id, string? status, int pageIndex, int pageSize);
}
