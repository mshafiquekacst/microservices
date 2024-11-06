using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<HttpCommandDataClient> _logger;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration config, ILogger<HttpCommandDataClient> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                               JsonSerializer.Serialize(plat),
                               Encoding.UTF8,
                               "application/json");

            var response = await _httpClient.PostAsync($"{_config["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("--> Sync POST to Command Service was OK!");
            }
            else
            {
                _logger.LogError("--> Sync POST to Command Service was NOT OK!");
            }
        }
    }
}
