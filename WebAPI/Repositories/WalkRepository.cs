using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models.Domain;
using WebAPI.Repositories.IRepository;
using WebAPI.Repository;

namespace WebAPI.Repositories
{
    public class WalkRepository : Repository<Walk>, IWalkRepository
    {
        private readonly WalksDbContext _db;

        public WalkRepository(WalksDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk entity)
        {
            if(!await _db.Walks.AnyAsync(obj => obj.Id == id))
            {
                return null;
            }

            entity.Id = id;

            return _db.Walks.Update(entity).Entity;
        }
    }
}