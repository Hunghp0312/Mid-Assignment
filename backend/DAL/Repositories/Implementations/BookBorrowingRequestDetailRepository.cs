using DAL.Context;
using DAL.Entity;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories.Implementations;

public class BookBorrowingRequestDetailRepository : Repository<BookBorrowingRequestDetail>, IBookBorrowingRequestDetailRepository
{
    public BookBorrowingRequestDetailRepository(AppDbContext context) : base(context)
    {
    }
}

