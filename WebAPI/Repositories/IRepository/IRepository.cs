using System.Linq.Expressions;

namespace WebAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task SaveAsync();

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                        string? includeProperties = null,
                                        bool isTracking = false,
                                        int pageNumber = 1,
                                        int pageSize = 1000);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null,
                                string? includeProperties = null,
                                bool isTracking = false);

        Task<T?> GetByIdAsync(Guid id);

        Task<T> CreateAsync(T entity);

        Task<T?> DeleteAsync(Guid id);
    }
}