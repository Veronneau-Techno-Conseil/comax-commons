using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI
{
    public static class OrleansClientSetup
    {
        public static void SetupOrleansClient(this IServiceCollection collection)
        {
            // Add Orleans Service 
            //Consider refactoring the code
            collection.AddSingleton<IClusterClient>(provider => {
                
                var clientBuilder = new ClientBuilder()
                    .Configure<ClusterOptions>(options => {
                        options.ClusterId = "dev";
                        options.ServiceId = "CoreBlog";
                    })
                    .ConfigureApplicationParts(parts => {
                        parts.AddFromApplicationBaseDirectory();
                    });

                
                clientBuilder = clientBuilder.UseLocalhostClustering();
                
                clientBuilder.AddSimpleMessageStreamProvider("SMSProvider");
                var client = clientBuilder.Build();
                var reset = new ManualResetEvent(false);

                client.Connect(RetryFilter).ContinueWith(task => {
                    reset.Set();

                    return Task.CompletedTask;
                });

                reset.WaitOne();

                return client;

                async Task<bool> RetryFilter(Exception exception)
                {
                    provider.GetService<ILogger>()?.LogWarning(
                        exception,
                        "Exception while attempting to connect to Orleans cluster"
                    );

                    await Task.Delay(TimeSpan.FromSeconds(2));

                    return true;
                }
            });
        }
    }
}
