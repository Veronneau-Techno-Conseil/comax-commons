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

namespace CommunAxiom.Commons.Client.Silo
{
    public static class PilotSilo
    {
        static ISiloHost _silo;
        public static bool IsSiloStarted { get; private set; }

        public static async Task StartSilo()
        {
            if (IsSiloStarted)
            {
                return;
            }
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .SetClustering()
                .SetConfiguration()
                .ConfigureServices(services =>
                {
                    var configs = new Orleans.Security.IdentityServer4Info("https://localhost:5001/", "org1_node1", "846B62D0-DEF9-4215-A99D-86E6B8DAB342", "org1");
                    services.AddOrleansClusteringAuthorization(configs,
                        config =>
                        {
                            config.ConfigureAuthorizationOptions = CommunAxiom.Commons.Client.Contracts.Auth.Configuration.ConfigurePolicyOptions;
                            config.TracingEnabled = true;
                        });
                    //services.Remove(new ServiceDescriptor(typeof(IdentityServer4Info), configs));
                    services.AddTransient<IdentityServer4Info>(services =>
                    {
                        var gf = services.GetRequiredService<IGrainFactory>();
                        var actGrain = gf.GetGrain<IAccount>(Guid.Empty);
                        var act = actGrain.GetDetails().GetAwaiter().GetResult();
                        var cfg = services.GetRequiredService<IConfiguration>();
                        OIDCSettings authSettings = new OIDCSettings();
                        cfg.Bind(Sections.OIDCSection, authSettings);
                        return new IdentityServer4Info(authSettings.Authority, act.ClientID, act.ClientSecret, authSettings.Scopes);
                    });
                    //register singleton services for each grain/interface
                    services.AddSingleton<IAccount, Accounts>();
                    services.AddSingleton<IDatasource, Datasources>();
                    services.AddSingleton<IDataTransfer, DataTransfer>();
                    services.AddSingleton<IIngestion, Ingestions>();
                    services.AddSingleton<IPortfolio, Portfolios>();
                    services.AddSingleton<IProject, Projects>();
                    services.AddSingleton<IReplication, Replications>();

                })
                .AddSimpleMessageStreamProvider("SMSProvider")
                .ConfigureEndpoints(siloPort: 7717, gatewayPort: 30000)
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                // Configure connectivity
                .UseDashboard()
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                //configure application parts for each grain
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Accounts).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

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
                _silo.Dispose();
                _silo = null;
            }
        }
    }
}
