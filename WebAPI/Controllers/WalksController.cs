using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using WebAPI.CustomActionFilters;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;
using WebAPI.Repositories.IRepository;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWalkRepository _walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            _mapper = mapper;
            _walkRepository = walkRepository;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            var walkDomainModel = _mapper.Map<Walk>(addWalkRequestDto);

            walkDomainModel = await _walkRepository.CreateAsync(walkDomainModel);

            await _walkRepository.SaveAsync();

            return Ok(_mapper.Map<WalkDto>(walkDomainModel));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            Func<Walk, object>? orderBy = sortBy switch
            {
                "name" => obj => obj.Name,
                "lengthInKm" => obj => obj.LengthInKm,
                "region" => obj => obj.Region,
                "difficulty" => obj => obj.Difficulty,
                _ => null
            };

            Func<IQueryable<Walk>, IOrderedEnumerable<Walk>>? order = null;

            if(orderBy is not null)
            {
                order = isAscending
                    ? list => list.OrderBy(orderBy)
                    : list => list.OrderByDescending(orderBy);
            }

            var er = double.TryParse(filterQuery!, out double length);

            Expression<Func<Walk, bool>>? where = filterOn switch
            {
                "name" => obj => obj.Name.Contains(filterQuery!, StringComparison.OrdinalIgnoreCase),
                "lengthInKm" => obj => obj.LengthInKm == (er ? length : -1),
                "region" => obj => obj.Region.Name.Contains(filterQuery!, StringComparison.OrdinalIgnoreCase),
                "difficulty" => obj => obj.Difficulty.Name.Contains(filterQuery!, StringComparison.OrdinalIgnoreCase),
                _ => null
            };

            var walksDomainModel = await _walkRepository.GetAllAsync(
                filter: where,
                orderBy: (Func<IQueryable<Walk>, IOrderedQueryable<Walk>>?)order,
                includeProperties: "Difficulty,Region",
                pageNumber: pageNumber,
                pageSize: pageSize
                );

            return Ok(_mapper.Map<List<WalkDto>>(walksDomainModel));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await _walkRepository.FirstOrDefaultAsync(obj => obj.Id == id, "Difficulty,Region");

            if(walkDomainModel == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<WalkDto>(walkDomainModel));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walkDomainModel = _mapper.Map<Walk>(updateWalkRequestDto);

            walkDomainModel = await _walkRepository.UpdateAsync(id, walkDomainModel);

            if(walkDomainModel == null)
            {
                return NotFound();
            }

            await _walkRepository.SaveAsync();

            return Ok(_mapper.Map<WalkDto>(walkDomainModel));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedWalkDomainModel = await _walkRepository.DeleteAsync(id);

            if(deletedWalkDomainModel == null)
            {
                return NotFound();
            }

            await _walkRepository.SaveAsync();

            return Ok(_mapper.Map<WalkDto>(deletedWalkDomainModel));
        }
    }
}