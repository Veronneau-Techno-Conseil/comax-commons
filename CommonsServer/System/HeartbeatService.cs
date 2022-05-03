using CommunAxiom.Commons.Client.Silo.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public class HeartbeatService : BackgroundService
    {
        private ClusterManagement clusterManagement;
        public HeartbeatService(IServiceProvider serviceProvider)
        {
            clusterManagement = serviceProvider.GetService<ClusterManagement>();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await clusterManagement.SetSilo(Silos.Pilot);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await clusterManagement.Heartbeat();
                await Task.Delay(1000);
            }
        }
    }
}
