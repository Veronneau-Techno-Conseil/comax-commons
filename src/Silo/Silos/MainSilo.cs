using System;
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
using CommunAxiom.Commons.Client.Contracts.Grains.Dispatch;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Grains.DispatchGrain;
using CommunAxiom.Commons.Client.Grains.StorageGrain;
using CommunAxiom.Commons.Ingestion;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Extentions;
using CommunAxiom.Commons.Ingestion.Ingestor;

namespace CommunAxiom.Commons.Client.Silo
{
    internal static class MainSilo
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
                .UseDashboard()
                .ConfigureServices(services =>
                {
                    ConfigureIdentitty(services);

                    services.SetStorage(conf);
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
                    services.AddSingleton<IPortfolio, Portfolios>();
                    services.AddSingleton<PortfolioRepo, PortfolioRepo>();
                    services.AddSingleton<PortfolioBusiness, PortfolioBusiness>();
                    services.AddSingleton<IProject, Projects>();
                    services.AddSingleton<IReplication, Replications>();
                    services.AddSingleton<SchedulerBusiness, SchedulerBusiness>();
                    services.AddSingleton<SchedulerRepo, SchedulerRepo>();
                    services.AddSingleton<IDispatch, Dispatch>();       
                    services.AddSingleton<IBroadcast, Grains.BroadcastGrain.Broadcast>();

                    services.AddSingleton<ISettingsProvider, SiloSettingsProvider>();
                    services.AddSingleton<IClaimsPrincipalProvider, OIDCClaimsProvider>();
                    services.AddSingleton<IIncomingGrainCallFilter, AccessControlFilter>();

                    services.AddSingleton<Importer, Importer>();

                    services.AddSingleton<IDataSourceFactory, DataSourceFactory>();
                    services.AddSingleton<IIngestorFactory, IngestorFactory>();

                    
                    // data sources

                    services.AddScoped<TextDataSourceReader>();
            
                    services.AddTransient<Func<DataSourceType, IDataSourceReader>>(provider => key =>
                    {
                        switch (key)
                        {
                            case DataSourceType.FILE:
                                return provider.GetService<TextDataSourceReader>();
                            case DataSourceType.API:
                                return null;
                        }

                        return null;
                    });
                    
                })
                //configure application parts for each grain
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Accounts).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Dataset).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Datasources).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DataTransfer).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Ingestions).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Portfolios).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Projects).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Replications).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Grains.BroadcastGrain.Broadcast).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Dispatch).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Scheduler).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(StorageGrain).Assembly).WithReferences());
            
            var silo = builder.Build();
            await silo.StartAsync();
            _silo = silo;
            IsSiloStarted = true;
        }

        static void ConfigureIdentitty(IServiceCollection services)
        {
            
            services.AddTransient(services => SiloShared.Conf.OIDCConfig.Config);
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
