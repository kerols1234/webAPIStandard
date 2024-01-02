using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models.Domain;
using WebAPI.Repositories.IRepository;
using WebAPI.Repository;

namespace WebAPI.Repositories
{
    public class RegionRepository : Repository<Region>, IRegionRepository
    {
        private readonly WalksDbContext _db;

        public RegionRepository(WalksDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Region?> UpdateAsync(Guid id, Region entity)
        {
            if(!await _db.Regions.AnyAsync(obj => obj.Id == id))
            {
                return null;
            }

            entity.Id = id;

            return _db.Regions.Update(entity).Entity;
        }
    }
}