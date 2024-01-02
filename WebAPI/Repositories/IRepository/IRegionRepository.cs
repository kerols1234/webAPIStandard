using WebAPI.Models.Domain;
using WebAPI.Repository.IRepository;

namespace WebAPI.Repositories.IRepository
{
    public interface IRegionRepository : IRepository<Region>
    {
        Task<Region?> UpdateAsync(Guid id, Region entity);
    }
}