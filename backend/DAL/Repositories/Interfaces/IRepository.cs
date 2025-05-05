namespace DAL.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<int> GetCountAsync();
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    T Update(T entity);
    void Remove(T Entity);
    Task SaveChangesAsync();
}
