using CommunAxiom.Commons.Client.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo.System
{
    public class ClusterManagement
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private SystemListener systemListener;
        public Silos CurrentSilo { get; private set; } = Silos.Unspecified;

        public ClusterManagement(IServiceProvider serviceProvider, ILogger<ClusterManagement> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task Heartbeat()
        {
            if (await Services.ClusterManagement.IsServiceAuthSet())
            {
                logger.LogInformation("Service auth already set, switching to main silo");
                await Services.ClusterManagement.SetSilo(Silos.Main);
            }
            else
            {
                await Services.ClusterManagement.SetSilo(Silos.Pilot);
            }
        }

        public async Task SetSilo(Silos requiredSilo)
        {
            if (requiredSilo == CurrentSilo) return;
            switch (CurrentSilo)
            {
                case Silos.Unspecified:
                    break;
                case Silos.Bootstrap:
                    break;
                case Silos.Pilot:
                    logger.LogInformation("Stopping Pilot silo...");
                    await PilotSilo.StopSilo();
                    break;
                case Silos.Main:
                    logger.LogInformation("Stopping Main silo...");
                    await MainSilo.StopSilo();
                    break;
            }
            Services.ResetClient();

            switch (requiredSilo)
            {
                case Silos.Unspecified:
                case Silos.Bootstrap:
                    break;
                case Silos.Pilot:
                    logger.LogInformation("Starting Pilot silo...");
                    await PilotSilo.StartSilo();
                    CurrentSilo = Silos.Pilot;
                    break;
                case Silos.Main:
                    logger.LogInformation("Starting Main silo...");
                    await MainSilo.StartSilo();
                    break;
                    CurrentSilo = Silos.Main;
            }

            systemListener = new SystemListener(this);
            await systemListener.Listen();
        }

        public async Task<bool> IsServiceAuthSet()
        {
            var client = serviceProvider.GetService<ICommonsClusterClient>();
            var state = await client.GetAccount().CheckState();
            return state == Contracts.Account.AccountState.CredentialsSet;
        }
    }
}
