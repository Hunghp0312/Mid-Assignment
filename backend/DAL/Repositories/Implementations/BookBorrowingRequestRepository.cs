using DAL.Context;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Implementations;

public class BookBorrowingRequestRepository : Repository<BookBorrowingRequest>, IBookBorrowingRequestRepository
{
    public BookBorrowingRequestRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<int> GetUserTotalRequestInMonth(Guid requestorId)
    {
        var currentDate = DateTime.UtcNow;
        var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        var totalRequests = await _dbSet
            .Where(r => r.RequestorId == requestorId && r.RequestDate >= startOfMonth 
            && r.RequestDate <= endOfMonth && r.Status != BookBorrowingRequestStatus.Rejected)
            .CountAsync();
        return totalRequests;
    }
    public async Task<BookBorrowingRequest?> GetByIdWithDetailsAsync(Guid id)
    {
        var bookBorrowingRequest = await _dbSet
            .Include(r => r.BookBorrowingRequestDetails)
            .ThenInclude(d => d.Book)
            .FirstOrDefaultAsync(r => r.Id == id);
        return bookBorrowingRequest;
    }
    public async Task<(IEnumerable<BookBorrowingRequest>, int)> GetAllWithPaginationAsync(string? status, int pageIndex, int pageSize)
    {
        var query = _dbSet.AsQueryable();
        if (Enum.TryParse<BookBorrowingRequestStatus>(status, ignoreCase: true, out var statusEnum))
        {
            query = query.Where(r => r.Status == statusEnum);
        }
        var totalCount = await query.CountAsync();
        var bookBorrowingRequests = await query
            .Include(r => r.Requestor)
            .Include(r => r.Approver)
            .Include(r => r.BookBorrowingRequestDetails)
            .ThenInclude(d => d.Book)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (bookBorrowingRequests, totalCount);
    }
}
