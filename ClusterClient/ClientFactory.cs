using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
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

namespace ClusterClient
{
    public class ClientFactory : ICommonsClientFactory
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
                    .AddOutgoingGrainCallFilter(serviceProvider.GetService<SecureTokenOutgoingFilter>())
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

        public async Task WithClusterClient(Func<ICommonsClusterClient, Task> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            using (var client = builder.Build())
            {
                await client.Connect(GetRetryFilter(c));
                var cl = new Client(client);
                await action(cl);
                await cl.Close();
            }
        }
        public async Task<TResult> WithClusterClient<TResult>(Func<ICommonsClusterClient, Task<TResult>> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            using (var client = builder.Build())
            {
                await client.Connect(GetRetryFilter(c));
                var cl = new Client(client);
                var res = await action(cl);
                await cl.Close();
                return res;
            }
        }

        public async Task<ICommonsClusterClient> WithUnmanagedClient(Func<ICommonsClusterClient, Task> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            var client = builder.Build();
            await client.Connect(GetRetryFilter(c));
            var cl = new Client(client);
            await action(cl);
            return cl;
        }

        public async Task<(ICommonsClusterClient, TResult)> WithUnmanagedClient<TResult>(Func<ICommonsClusterClient, Task<TResult>> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            var client = builder.Build();
            await client.Connect(GetRetryFilter(c));
            var cl = new Client(client);
            return (cl, await action(cl));
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
