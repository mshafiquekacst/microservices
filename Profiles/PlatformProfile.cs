using PlatformService.Dtos;
using PlatformService.Models;
using AutoMapper;
namespace PlatformService.Profiles
{
    public class PlatformProfile:Profile
    {
        public PlatformProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
        }
    }
}
