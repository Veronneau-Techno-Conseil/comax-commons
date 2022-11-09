using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.SiloShared.System
{
    public class HeartbeatService : BackgroundService
    {
        private IClusterManagement clusterManagement;
        public HeartbeatService(IServiceProvider serviceProvider)
        {
            clusterManagement = serviceProvider.GetService<IClusterManagement>();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            await clusterManagement.SetSilo(Silos.Main);
            
            while (!stoppingToken.IsCancellationRequested)
            {

                await clusterManagement.Heartbeat();
                await Task.Delay(1000);
            }
        }
        
    }
}
