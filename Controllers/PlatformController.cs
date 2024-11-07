using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo platformRepo;
        private readonly IMapper mapper;
        private readonly ICommandDataClient commandDataClient;
        private readonly IMessageBusClient messageBusClient;

        public PlatformController(IPlatformRepo platformRepo, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            this.platformRepo = platformRepo;
            this.mapper = mapper;
            this.commandDataClient = commandDataClient;
            this.messageBusClient = messageBusClient;
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
        public async Task<ActionResult> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platform = this.mapper.Map<Platform>(platformCreateDto);
            platformRepo.CreatePlatform(platform);
            platformRepo.SaveChanges();
            var platformReadDto = this.mapper.Map<PlatformReadDto>(platform);
            await this.commandDataClient.SendPlatformToCommand(this.mapper.Map<PlatformReadDto>(platform));
            try
            {
                
                Console.WriteLine($"--> Starting sending data to command service");
                var platformDto = this.mapper.Map<PlatformPublishDto>(platformReadDto);
                platformDto.Event = "Platform_Published";   
                this.messageBusClient.PublishNewPlatform(platformDto);
                //this.messageBusClient.PublishNewPlatform();

                Console.WriteLine($"--> Sent data to command service");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send publish to message bus: {ex.Message}");
            }
            
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
