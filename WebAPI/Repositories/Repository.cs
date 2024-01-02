using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI.Data;
using WebAPI.Repository.IRepository;

namespace WebAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly WalksDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(WalksDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);

            return entity;
        }

        public async Task<T?> DeleteAsync(Guid id)
        {
            var result = await dbSet.FindAsync(id);

            if(result is not null)
            {
                dbSet.Remove(result);
            }

            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
                                                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                    string? includeProperties = null,
                                                    bool isTracking = false,
                                                    int pageNumber = 1,
                                                    int pageSize = 1000)
        {
            IQueryable<T> query = dbSet;

            if(filter is not null)
            {
                query = query.Where(filter);
            }

            if(includeProperties is not null)
            {
                foreach(var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if(!isTracking)
            {
                query = query.AsNoTracking();
            }

            if(orderBy is not null)
            {
                query = orderBy(query);
            }

            var skipResults = (pageNumber - 1) * pageSize;

            return await query.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null,
                                string? includeProperties = null,
                                bool isTracking = false)
        {
            IQueryable<T> query = dbSet;

            if(filter is not null)
            {
                query = query.Where(filter);
            }

            if(includeProperties is not null)
            {
                foreach(var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if(!isTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}