using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CustomActionFilters;
using WebAPI.Data;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;
using WebAPI.Repositories.IRepository;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly WalksDbContext _dbContext;
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RegionsController> _logger;

        public RegionsController(WalksDbContext dbContext,
            IRegionRepository regionRepository,
            IMapper mapper,
            ILogger<RegionsController> logger)
        {
            _dbContext = dbContext;
            _regionRepository = regionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        //[Authorize(Roles = "Reader")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regionsDomain = await _regionRepository.GetAllAsync();

            return Ok(_mapper.Map<List<RegionDto>>(regionsDomain));
        }

        //[Authorize(Roles = "Reader")]
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var regionDomain = await _regionRepository.GetByIdAsync(id);

            if(regionDomain == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RegionDto>(regionDomain));
        }

        //[Authorize(Roles = "Writer")]
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            var regionDomainModel = _mapper.Map<Region>(addRegionRequestDto);

            regionDomainModel = await _regionRepository.CreateAsync(regionDomainModel);

            await _regionRepository.SaveAsync();

            var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        //[Authorize(Roles = "Writer")]
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var regionDomainModel = _mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await _regionRepository.UpdateAsync(id, regionDomainModel);

            if(regionDomainModel == null)
            {
                return NotFound();
            }

            await _regionRepository.SaveAsync();

            return Ok(_mapper.Map<RegionDto>(regionDomainModel));
        }

        //[Authorize(Roles = "Writer,Reader")]
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await _regionRepository.DeleteAsync(id);

            if(regionDomainModel == null)
            {
                return NotFound();
            }

            await _regionRepository.SaveAsync();

            return Ok(_mapper.Map<RegionDto>(regionDomainModel));
        }
    }
}