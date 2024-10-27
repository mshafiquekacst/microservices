using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo platformRepo;
        private readonly IMapper mapper;

        public PlatformController( IPlatformRepo platformRepo, IMapper mapper)
        {
            this.platformRepo = platformRepo;
            this.mapper = mapper;
        }
        [HttpGet]
        public ActionResult GetAllPlatforms()
        {
            var platforms = platformRepo.GetAllPlatforms();
            return Ok(this.mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }
        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult GetPlatformById(int id)
        {
            var platform = platformRepo.GetPlatformById(id);
            if (platform != null)
            {
                return Ok(this.mapper.Map<PlatformReadDto>(platform));
            }
            return NotFound();
        }
        [HttpPost]
        public ActionResult CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platform = this.mapper.Map<Platform>(platformCreateDto);
            platformRepo.CreatePlatform(platform);
            platformRepo.SaveChanges();
            var platformReadDto = this.mapper.Map<PlatformReadDto>(platform);
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
