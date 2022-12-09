
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public class AgentSyncService : BackgroundService
    {
        private readonly ILogger<AgentSyncService> _logger;
        private readonly ClientManager _clientManager;
        public AgentSyncService(ILogger<AgentSyncService> logger, ClientManager clientManager)
        {
            _logger = logger;
            _clientManager = clientManager;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_clientManager.IsConnectionActive)
                    {
                        AgentIntegration.IsConnected = false;
                        continue;
                    }
                    AgentIntegration.IsConnected = true;
                    var jobs = AgentIntegration.Jobs;
                    foreach(var j in jobs)
                    {
                        if (_clientManager.Client != null)
                        {
                            await j.IAmAlive(_clientManager.Client);
                        }
                    }
                }
                catch(Exception ex) {
                    _logger.LogError(ex, "Error running agent sync");
                }
                await Task.Delay(500);
            }
        }
    }
}
