using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using Orleans.Security.Clustering;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using CommunAxiom.Commons.Client.Grains.AccountGrain;
using DatasourceGrain;
using DataTransferGrain;
using IngestionGrain;
using PortfolioGrain;
using ProjectGrain;
using ReplicationGrain;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Replication;

namespace CommunAxiom.Commons.Client.Silo
{
    class Program
    {

        static ISiloHost silo;
        static readonly ManualResetEvent _siloStopped = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.WriteLine("Orleans Silo is Launching");
            RunMainAsync().GetAwaiter().GetResult();
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var silo = await StartSilo();
                
                
                Console.WriteLine("\n\n The Silo is Up\n\n");
                Console.ReadLine();
                await StopSilo();

                // Wait for the silo to completely shutdown before exiting. 
                _siloStopped.WaitOne();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .ConfigureServices(services =>
                {
                    services.AddOrleansClusteringAuthorization(new Orleans.Security.IdentityServer4Info("https://localhost:5001/", "org1_node1", "846B62D0-DEF9-4215-A99D-86E6B8DAB342", "org1"),
                        config =>
                        {
                            config.ConfigureAuthorizationOptions = CommunAxiom.Commons.Client.Contracts.Auth.Configuration.ConfigureOptions;
                            config.TracingEnabled = true;
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
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Datasources).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(DataTransfer).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Ingestions).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Portfolios).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Projects).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Replications).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());
                //to add the redis Grain directory

            var silo = builder.Build();
            await silo.StartAsync();
            return silo;
        }

        static async Task StopSilo()
        {
            await silo.StopAsync();
            _siloStopped.Set();
        }
    }
}
