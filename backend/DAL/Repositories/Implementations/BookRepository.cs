using Common.Parameters;
using DAL.Context;
using DAL.Entity;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Implementations;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<(IEnumerable<Book>, int)> GetAllWithPaging(BookFilterParams filterParams, int pageIndex, int pageSize)
    {
        var result = _dbSet.AsNoTracking().Include(x => x.Category)
            .AsQueryable();
        if (!string.IsNullOrEmpty(filterParams.Query))
        {
            result = result.Where(b =>
            b.Title.Contains(filterParams.Query) ||
            b.Author.Contains(filterParams.Query) ||
            (b.Category != null && b.Category.Name.Contains(filterParams.Query)));
        }
        if (filterParams.Available.HasValue)
        {
            if (filterParams.Available.Value)
            {
                result = result.Where(b => b.Available > 0);
            }
            else
            {
                result = result.Where(b => b.Available == 0);
            }
        }
        //if (filterParams.Rating.HasValue)
        //{
        //    result = result.Where(b => b.AverageRating >= filterParams.Rating);
        //}
        if (filterParams.CategoryId.HasValue)
        {
            result = result.Where(b => b.CategoryId == filterParams.CategoryId);
        }
        var count = await result.CountAsync();
        var books = await result
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
        return (books, count);
    }
}
