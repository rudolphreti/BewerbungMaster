using Microsoft.Extensions.Configuration;

namespace BewerbungMasterApp.Services
{
    public class ConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> JobTypesWithoutPosition => _configuration.GetSection("JobTypesWithoutPosition").Get<List<string>>() ?? new List<string>();
    }
}