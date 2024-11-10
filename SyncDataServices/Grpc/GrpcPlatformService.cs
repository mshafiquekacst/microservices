using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepo platformRepo;
        //private readonly CommandService.CommandServiceClient _client;
        public GrpcPlatformService(IMapper mapper, IPlatformRepo platformRepo)
        {
            _mapper = mapper;
            this.platformRepo = platformRepo;
          //  _client = new CommandService.CommandServiceClient(GrpcChannel.ForAddress("https://localhost:5001"));
        }
        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = platformRepo.GetAllPlatforms();
            foreach (var plat in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));
            }
            return Task.FromResult(response);
        }
    }
}
