using CommunAxiom.Commons.Client.Grains.AccountGrain;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using Orleans.Security;
using Orleans.Security.Clustering;

namespace CommunAxiom.Commons.Client.Silo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            RunMainAsync();
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

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
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                }) 
                // Configure connectivity
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                //.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Accounts).Assembly).WithReferences())
                .ConfigureServices(services =>
                {

                    services.AddOrleansClusteringAuthorization(new Orleans.Security.IdentityServer4Info("https://localhost:5001/", "org1_node1", "846B62D0-DEF9-4215-A99D-86E6B8DAB342", "org1"),
                        config =>
                        {
                            config.ConfigureAuthorizationOptions = CommunAxiom.Commons.Client.Contracts.Auth.Configuration.ConfigureOptions;
                            config.TracingEnabled = true;
                        });
                })
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
