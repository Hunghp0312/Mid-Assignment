using DAL.Context;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Implementations;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    } 
    public async Task<IEnumerable<Category>> GetAllWithPaging(int pageIndex, int pageSize)
    {
        var categories = await _dbSet
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return categories;
    }
}
