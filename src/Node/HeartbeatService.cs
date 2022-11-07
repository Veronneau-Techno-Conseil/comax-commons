using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator
{
    public class HeartbeatService : BackgroundService
    {
        private readonly ILogger<HeartbeatService> logger;
        public HeartbeatService(IServiceProvider serviceProvider, ILogger<HeartbeatService> logger)
        {
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int silocrashed = 0;
            //Start silo host
            while (!stoppingToken.IsCancellationRequested)
            {
                if (MainSilo.IsSiloStarted)
                    continue;
                else
                    await MainSilo.StartSilo();
                await MainSilo.Host.Stopped;
                if (!stoppingToken.IsCancellationRequested)
                {
                    silocrashed++;
                    logger.LogWarning($"Orchestrator silo crashed {silocrashed} time(s)");
                    if (silocrashed > 9)
                        break;
                    continue;
                }
                else
                    break;
            }
        }
    }
}
