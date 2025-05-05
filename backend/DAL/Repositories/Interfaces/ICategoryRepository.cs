using DAL.Entity;

namespace DAL.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithPaging(int pageIndex, int pageSize);
}
