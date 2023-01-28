using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Auth;
using CommunAxiom.Commons.Client.Contracts.Dataset;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Replication;
using CommunAxiom.Commons.Client.Grains.AccountGrain;
using CommunAxiom.Commons.Client.Grains.DatasourceGrain;
using CommunAxiom.Commons.Client.Grains.IngestionGrain;
using CommunAxiom.Commons.Client.SiloShared;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using DatasetGrain;
using DataTransferGrain;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using PortfolioGrain;
using ProjectGrain;
using ReplicationGrain;
using SchedulerGrain;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Broadcast;
using CommunAxiom.Commons.Client.Contracts.Grains.DataStateMonitor;
using CommunAxiom.Commons.Client.Contracts.Grains.DateStateMonitorSupervisor;
using CommunAxiom.Commons.Client.Contracts.Grains.Dispatch;
using CommunAxiom.Commons.Client.Grains.DataStateMonitorGrain;
using CommunAxiom.Commons.Client.Grains.DateStateMonitorSupervisorGrain;
using CommunAxiom.Commons.Client.Grains.DispatchGrain;
using CommunAxiom.Commons.Client.Grains.StorageGrain;
using CommunAxiom.Commons.Ingestion.Extentions;
using CommunAxiom.Commons.Ingestion.Ingestor;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using Comax.Commons.Orchestrator.Client;
using CommunAxiom.Commons.Client.AgentService.OrchClient;
using CommunAxiom.Commons.Client.Grains.AgentGrain;
using CommunAxiom.Commons.Client.AgentService.Conf;
using System;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion;
using CommunAxiom.Commons.CommonsShared.EventMailboxGrain;
using Microsoft.Extensions.Configuration;
using Comax.Commons.CommonsShared.ApiMembershipProvider;
using CommunAxiom.Commons.Client.ClusterClient;
using Microsoft.AspNetCore.DataProtection;
using CommunAxiom.Commons.CommonsShared.UriRegistryGrain;
using CommunAxiom.Commons.Client.AgentService;
using Comax.Commons.Orchestrator.MailGrain;
using ExplorerGrain;
using CommunAxiom.Commons.Client.Grains.BroadcastGrain;

namespace CommunAxiom.Commons.Client.Silo
{
    public class MainSilo : IDisposable
    {
        ISiloHost _silo;

        public ISiloHost Host { get { return _silo; } }
        public IServiceProvider ServiceProvider
        {
            get
            {
                return this._silo.Services;
            }
        }

        public IConfiguration Configuration
        {
            get
            {
                return this._silo.Services.GetService<IConfiguration>();
            }
        }

        public bool IsSiloStarted { get; private set; }

        public async Task StartSilo(string configFile)
        {
            if (IsSiloStarted)
            {
                return;
            }
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .SetDefaults(out var conf, configFile ?? "./config.json")
                .UseDashboard()
                .ConfigureServices(services =>
                {
                    services.SetStorage(conf);

                    //services.CentralGrainSetup();

                    if (!conf["advertisedIp"].StartsWith("127.0.0.1"))
                        services.UseApiMembership(conf.GetSection("CommonsMembership"));

                    services.AddIngestion();

                    //register singleton services for each grain/interface
                    services.AddScoped<AccountRepo, AccountRepo>();
                    services.AddScoped<AccountBusiness, AccountBusiness>();
                    services.AddSingleton<IAccount, Accounts>();
                    services.AddSingleton<IAuthentication, AuthenticationWorker>();
                    services.AddSingleton<IDataset, Dataset>();
                    services.AddSingleton<IDatasource, Datasources>();
                    services.AddSingleton<IDataTransfer, DataTransfer>();
                    services.AddSingleton<IIngestion, Ingestions>();
                    services.AddSingleton<IPortfolio, PortfolioGrain.Portfolio>();
                    services.AddSingleton<PortfolioRepo, PortfolioRepo>();
                    services.AddSingleton<PortfolioBusiness, PortfolioBusiness>();
                    services.AddSingleton<IProject, Projects>();
                    services.AddSingleton<IReplication, Replications>();
                    services.AddSingleton<SchedulerBusiness, SchedulerBusiness>();
                    services.AddSingleton<SchedulerRepo, SchedulerRepo>();
                    services.AddSingleton<IDispatch, Dispatch>();
                    services.AddSingleton<IBroadcast, Grains.BroadcastGrain.Broadcast>();
                    services.AddSingleton<IDataStateMonitor, DataStateMonitor>();
                    services.AddSingleton<IDateStateMonitorSupervisor, DateStateMonitorSupervisor>();
                    services.AddSingleton<IAgentIntegration, AgentIntegrationAccessor>();
                    services.AddSingleton<ISettingsProvider, ConfigSettingsProvider>(sp =>
                    {
                        return new ConfigSettingsProvider("OIDC", conf);
                    });
                    services.AddSingleton<IClaimsPrincipalProvider, OIDCClaimsProvider>();
                    services.AddSingleton<IIncomingGrainCallFilter, AuthRequiredKeepAliveFilter>();
                    services.AddTransient<IOutgoingGrainCallFilter, SiloSourcedOutgoingFilter>();
                    services.AddSingleton<ITokenProvider, SiloTokenProvider>();
                    services.AddSingleton<AppIdProvider>();

                    services.AddSingleton<Importer>();

                    services.AddSingleton<IDataSourceFactory, DataSourceFactory>();
                    services.AddSingleton<IIngestorFactory, IngestorFactory>();

                    services.AddSingleton<IOrchestratorClientConfig, ClientConfig>();
                    services.AddSingleton<IOrchestratorClientFactory, Comax.Commons.Orchestrator.Client.ClientFactory>();
                   
                })
                //configure application parts for each grain
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Accounts).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Dataset).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Datasources).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DataTransfer).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Ingestions).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Portfolio).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Projects).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Replications).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Broadcast).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Dispatch).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Scheduler).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(StorageGrain).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DataStateMonitor).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DateStateMonitorSupervisor).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Agent).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(EventMailbox).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Mail).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(UriRegistry).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Explorer).Assembly).WithReferences())
                .AddStartupTask(async (sp, cancellation) =>
                {
                    var grainFactory = sp.GetRequiredService<IGrainFactory>();
                    var account = grainFactory.GetGrain<IAccount>(Guid.Empty);
                    var settingsProvider = sp.GetService<ISettingsProvider>();
                    var settings = await settingsProvider.GetOIDCSettings();

                    var detail = await account.GetDetails();
                    if (detail != null && !string.IsNullOrEmpty(detail.ClientID) && detail.ClientID != settings.ClientId)
                        throw new InvalidOperationException("Cannot launch silo while credentials don't match silo account");

                    if(detail == null)
                        detail = new AccountDetails();

                    detail.ClientID = settings.ClientId;

                    await account.Initialize(detail);                    

                    // Use the service provider to get the grain factory.
                    var agt = grainFactory.GetGrain<IAgent>(Guid.Empty);
                    await agt.EnsureStarted();
                });

            var silo = builder.Build();

            _ = silo.Stopped.ContinueWith(x =>
            {
                IsSiloStarted = false;
            });

            await silo.StartAsync();
            _silo = silo;
            IsSiloStarted = true;
        }


        public async Task StopSilo()
        {
            if (IsSiloStarted)
            {
                await _silo.StopAsync();
                _silo.Dispose();
                _silo = null;
            }
        }

        public void Dispose()
        {
            try
            {
                if (_silo != null)
                {
                    _silo.Dispose();
                }
            }
            catch { }
        }
    }
}
