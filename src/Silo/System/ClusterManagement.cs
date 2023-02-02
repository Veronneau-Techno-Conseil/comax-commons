using CommunAxiom.Commons.Client.AgentService.Conf;
using CommunAxiom.Commons.Client.AgentService.OrchClient;
using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.SiloShared.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo.System
{
    public class ClusterManagement : IClusterManagement, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly string _configFile;
        public bool SiloStarted { get; set; }
        private readonly MainSilo _mainSilo;

        public ClusterManagement(IServiceProvider serviceProvider, ILogger<ClusterManagement> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
            _configFile = configuration["ConfigPath"];
            _mainSilo = new MainSilo();
        }

        public async Task Heartbeat()
        {
            
            var client = _serviceProvider.GetService<ICommonsClientFactory>();
            await client.WithClusterClient(cc => cc.SetCredentials(_configuration));
            //var cm = _serviceProvider.GetService<ClientManager>();
            //await cm.InitializeTask;
            
            await StartSilo();
        }

        public async Task StartSilo()
        {
            if(!SiloStarted)
            {
                _logger.LogInformation("Starting Main silo...");
                await _mainSilo.StartSilo(_configFile);
                SiloStarted = true;
            }

        }

        public void Dispose()
        {
            _mainSilo.Dispose();
        }
    }
}
