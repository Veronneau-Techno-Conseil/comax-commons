using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using Orleans.Security.Clustering;
using Microsoft.Extensions.DependencyInjection;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using CommunAxiom.Commons.Client.Grains.AccountGrain;
using System.Threading;
using PortfolioGrain;
using ReplicationGrain;
using ProjectGrain;
using CommunAxiom.Commons.Client.Contracts.Replication;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using DatasourceGrain;
using DataTransferGrain;
using IngestionGrain;
using Microsoft.Extensions.Configuration;
using Orleans.Security;
using CommunAxiom.Commons.Client.Contracts.Configuration;
using CommunAxiom.Commons.Client.Contracts.Auth;
using CommunAxiom.Commons.Client.Contracts;

namespace CommunAxiom.Commons.Client.Silo
{
    public static class PilotSilo
    {
        static ISiloHost _silo;
        public static ISiloHost Host { get { return _silo; } }
        public static bool IsSiloStarted { get; private set; }

        public static async Task StartSilo()
        {
            if (IsSiloStarted)
            {
                return;
            }
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .SetDefaults(out var conf)
                .ConfigureServices(services =>
                {
                    services.SetStorage(conf);
                    services.AddSingleton<IAccount, Accounts>();
                    services.AddSingleton<IAuthentication, AuthenticationWorker>();

                })
                //configure application parts for each grain
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Accounts).Assembly).WithReferences());

            var silo = builder.Build();
            await silo.StartAsync();
            _silo = silo;
            IsSiloStarted = true;
        }

        public static async Task StopSilo()
        {
            if (IsSiloStarted)
            {
                await _silo.StopAsync();
                await _silo.DisposeAsync();
                _silo = null;
                IsSiloStarted=false;
            }
        }
    }
}
