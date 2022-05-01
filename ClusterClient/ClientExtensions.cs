using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using CommunAxiom.Commons.Client.Contracts;
using Orleans.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ClusterClient
{
    public static class ClientExtensions
    {
        public static void SetupOrleansClient(this IServiceCollection collection, bool transient = false)
        {
            if (transient)
            {
                collection.AddTransient<IClusterClient>(provider =>
                {
                    return Connect(provider).ConfigureAwait(false).GetAwaiter().GetResult();
                });
            }
            else
            {
                collection.AddSingleton<IClusterClient>(provider =>
                {
                    return Connect(provider).ConfigureAwait(false).GetAwaiter().GetResult();
                });
            }

            collection.AddTransient<ICommonsClusterClient, Client>();
        }
        static async Task<IClusterClient> Connect(IServiceProvider provider)
        {

            var clientBuilder = new ClientBuilder()
                    .Configure<ClusterOptions>(options => {
                        options.ClusterId = "dev";
                        options.ServiceId = "CoreBlog";
                    })
                    .ConfigureApplicationParts(parts => {
                        parts.AddFromApplicationBaseDirectory();
                    });

            clientBuilder = clientBuilder.UseLocalhostClustering(30000);
            clientBuilder.AddSimpleMessageStreamProvider(Constants.DefaultStream);

            var client = clientBuilder.Build();
            
            await client.Connect(RetryFilter);

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
        }
    }
}