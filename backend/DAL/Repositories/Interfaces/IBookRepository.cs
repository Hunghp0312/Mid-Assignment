using Common.Parameters;
using DAL.Entity;

namespace DAL.Repositories.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<(IEnumerable<Book>, int)> GetAllWithPaging(BookFilterParams filterParams,int pageIndex, int pageSize);
}
