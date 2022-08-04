using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteDbTests
{
    public class ClientFactory 
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;
        public ClientFactory(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        private IClientBuilder GetBuilder()
        {
            var b = new ClientBuilder()
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "0.0.1-a1";
                        options.ServiceId = "CommonsClientCluster";
                    })
                    .ConfigureApplicationParts(parts =>
                    {
                        parts.AddFromApplicationBaseDirectory();
                    }).UseLocalhostClustering(30000)
                    .AddSimpleMessageStreamProvider(Constants.DefaultStream);
            return b;
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                Counter c = new Counter() { Value = 4 };
                var builder = GetBuilder();
                using (var client = builder.Build())
                {
                    await client.Connect(GetRetryFilter(c));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task WithClusterClient(Func<IClusterClient, Task> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            using (var client = builder.Build())
            {
                await client.Connect(GetRetryFilter(c));
                await action(client);
                await client.Close();
            }
        }
        public async Task<TResult> WithClusterClient<TResult>(Func<IClusterClient, Task<TResult>> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            using (var client = builder.Build())
            {
                await client.Connect(GetRetryFilter(c));
                var res = await action(client);
                await client.Close();
                return res;
            }
        }

        public async Task<IClusterClient> WithUnmanagedClient(Func<IClusterClient, Task> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            var client = builder.Build();
            await client.Connect(GetRetryFilter(c));
            await action(client);
            return client;
        }

        public async Task<(IClusterClient, TResult)> WithUnmanagedClient<TResult>(Func<IClusterClient, Task<TResult>> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            var client = builder.Build();
            await client.Connect(GetRetryFilter(c));
            return (client, await action(client));
        }

        private Func<Exception, Task<bool>> GetRetryFilter(Counter c)
        {
            return async (Exception exception) =>
            {
                serviceProvider.GetService<ILogger>()?.LogWarning(
                    exception,
                    "Exception while attempting to connect to Orleans cluster"
                );
                if (c.Value == 5)
                {
                    return false;
                }
                await Task.Delay(TimeSpan.FromSeconds(2));
                c.Value++;
                return true;
            };
        }

        private class Counter
        {
            public int Value { get; set; } = 0;
        }

    }
}
