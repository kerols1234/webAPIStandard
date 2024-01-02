using WebAPI.Models.Domain;
using WebAPI.Repository.IRepository;

namespace WebAPI.Repositories.IRepository
{
    public interface IWalkRepository : IRepository<Walk>
    {
        Task<Walk?> UpdateAsync(Guid id, Walk walk);
    }
}