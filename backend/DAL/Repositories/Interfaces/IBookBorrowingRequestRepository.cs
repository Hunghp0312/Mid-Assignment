using DAL.Entity;

namespace DAL.Repositories.Interfaces;

public interface IBookBorrowingRequestRepository : IRepository<BookBorrowingRequest>
{
    Task<int> GetUserTotalRequestInMonth(Guid requestorId);
    Task<(IEnumerable<BookBorrowingRequest>, int)> GetAllWithPaginationAsync(string? status, int pageIndex, int pageSize);
}
