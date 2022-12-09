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
    public class ClusterManagement : IClusterManagement
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        //private SystemListener systemListener;
        private readonly IConfiguration configuration;
        public Silos CurrentSilo { get; private set; } = Silos.Unspecified;

        public ClusterManagement(IServiceProvider serviceProvider, ILogger<ClusterManagement> logger, IConfiguration configuration)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task Heartbeat()
        {
            //if (await IsServiceAuthSet())
            //{
            if (CurrentSilo != Silos.Main)
            {
                var client = serviceProvider.GetService<ICommonsClientFactory>();
                await client.WithClusterClient(cc => cc.SetCredentials(configuration));
                var cm = serviceProvider.GetService<ClientManager>();
                await cm.InitializeTask;

                logger.LogInformation("Service auth already set, switching to main silo");
            }
            await SetSilo(Silos.Main);
            //}
            //else
            //{
            //    await SetSilo(Silos.Pilot);
            //}
        }

        public async Task SetSilo(Silos requiredSilo)
        {
            if (requiredSilo == CurrentSilo) return;

            //if(CurrentSilo != Silos.Unspecified && this.systemListener != null)
            //{
            //    this.systemListener.Dispose();
            //    this.systemListener = null;   
            //}

            switch (CurrentSilo)
            {
                case Silos.Unspecified:
                    break;
                case Silos.Bootstrap:
                    break;
                //case Silos.Pilot:
                //    logger.LogInformation("Stopping Pilot silo...");
                //    await PilotSilo.StopSilo();
                //    break;
                case Silos.Main:
                    logger.LogInformation("Stopping Main silo...");
                    await MainSilo.StopSilo();
                    break;
            }

            switch (requiredSilo)
            {
                case Silos.Unspecified:
                case Silos.Bootstrap:
                    break;
                //case Silos.Pilot:
                //    logger.LogInformation("Starting Pilot silo...");
                //    await PilotSilo.StartSilo();
                //    CurrentSilo = Silos.Pilot;
                //    break;
                case Silos.Main:
                    logger.LogInformation("Starting Main silo...");
                    await MainSilo.StartSilo();
                    CurrentSilo = Silos.Main;
                    break;
            }

            //systemListener = new SystemListener(this, serviceProvider.GetRequiredService<ICommonsClientFactory>(), serviceProvider.GetRequiredService<IConfiguration>());
            //await systemListener.Listen();
        }

        //public async Task<bool> IsServiceAuthSet()
        //{
        //    var client = serviceProvider.GetService<ICommonsClientFactory>();
        //    var state = await client.WithClusterClient(cc=>cc.GetAccount().CheckState(false));
        //    return state == Contracts.Account.AccountState.CredentialsSet;
        //}

        //public async Task WaitForPortCleanup()
        //{
        //    int port = 8080; //<--- This is your value
        //    bool isAvailable = false;

        //    while (!isAvailable)
        //    {
        //        GC.WaitForPendingFinalizers();
        //        GC.Collect();
        //        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        //        TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
        //        var conn = tcpConnInfoArray.FirstOrDefault(x => {
        //            bool ret = true;
        //            ret &= x != null;
        //            ret &= x.State == TcpState.Established;
        //            ret &= x.LocalEndPoint != null;
        //            ret &= x.LocalEndPoint.Port == port;
        //            return ret;
        //            });
        //        isAvailable = conn == null;
        //        await Task.Delay(250);
        //    }
        //}
    }
}
