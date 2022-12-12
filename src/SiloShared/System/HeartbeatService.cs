using CommunAxiom.Commons.Client.Contracts.ComaxSystem;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.SiloShared.System
{
    public class HeartbeatService : BackgroundService
    {
        private IClusterManagement clusterManagement;
        private IServiceProvider serviceProvider;
        private IConfiguration configuration;
        private ILogger logger;
        public HeartbeatService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<HeartbeatService> logger)
        {
            clusterManagement = serviceProvider.GetService<IClusterManagement>();
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            await clusterManagement.SetSilo(Silos.Main);



            logger.LogInformation("Service auth already set, switching to main silo");



            while (!stoppingToken.IsCancellationRequested)
            {

                await clusterManagement.Heartbeat();
                await Task.Delay(1000);
            }
        }

    }
}
