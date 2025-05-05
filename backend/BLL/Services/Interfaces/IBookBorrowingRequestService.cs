using BLL.DTOs.BookBorrowingRequestDTO;
using BLL.DTOs.GenericDTO;
using DAL.Entity;

namespace BLL.Services.Interfaces;

public interface IBookBorrowingRequestService
{
    Task AddBookBorrowingRequest(BookBorrowingRequestRequestDTO bookBorrowing, Guid requestorId);
    Task ApproveBookBorrowingRequest(Guid approverId, Guid bookBorrowingRequestId);
    Task RejectBookBorrowingRequest(Guid approverId, Guid bookBorrowingRequestId);
    Task<PaginatedList<BookBorrowingRequestResponseDTO>> GetAllWithPaginationAsync(string? status, int pageIndex, int pageSize);
    Task<IEnumerable<BookBorrowingRequestResponseDTO>> GetAllAsync();
}
