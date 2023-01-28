using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Silo;
using CommunAxiom.Commons.Orleans.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net.Http.Headers;
using Comax.Commons.CommonsShared.ApiMembershipProvider;

namespace CommunAxiom.Commons.Client.ClusterClient
{
    public class ClientFactory : ICommonsClientFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _configFile;
        private readonly ILogger _logger;
        public ClientFactory(IServiceProvider serviceProvider, string configFile)
        {
            _serviceProvider = serviceProvider;
            _configFile = configFile;
            _logger = serviceProvider.GetService<ILogger<ClientFactory>>();
        }

        private IClientBuilder GetBuilder()
        {
            ConfigurationBuilder cb = new ConfigurationBuilder();
            cb.AddInMemoryCollection(DefaultConfigs.Configs);
            cb.AddJsonFile(_configFile);
            cb.AddEnvironmentVariables();
            IConfiguration config = cb.Build();

            var b = new ClientBuilder()
                    .ConfigureAppConfiguration(cfg =>
                    {
                        cfg.AddInMemoryCollection(DefaultConfigs.Configs);
                        cfg.AddJsonFile(_configFile);
                        cfg.AddEnvironmentVariables();

                    })
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "0.0.1-a1";
                        options.ServiceId = "CommonsClientCluster";
                    });
            b.ConfigureApplicationParts(parts =>
                    {
                        parts.AddFromApplicationBaseDirectory();
                    });

            if (config["client_mode"] == "local")
            {
                b.UseLocalhostClustering(int.TryParse(config["client_port"], out int port) ? port : 30000);
            }
            else
            {
                b.UseApiClustering<TokenProviderClientFactory>(mo =>
                {
                    config.GetSection("CommonsMembership").Bind(mo);
                });
            }
            b.AddOutgoingGrainCallFilter(_serviceProvider.GetService<IOutgoingGrainCallFilter>())
                .AddSimpleMessageStreamProvider(CommunAxiom.Commons.Orleans.OrleansConstants.Streams.DefaultStream);
            return b;
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                Counter c = new Counter() { Value = 1 };
                var builder = GetBuilder();
                using (var client = builder.Build())
                {
                    await client.Connect(GetRetryFilter(c));
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Source == "Microsoft.Extensions.DependencyInjection")
                    throw;
                _logger.LogError(ex, "Error testing connection");
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
                var logger = _serviceProvider.GetService<ILogger<Client>>();
                var cl = new Client(client, logger);
                try
                {
                    await action(cl);
                }
                finally
                {
                    await cl.Close();
                }
            }
        }
        public async Task<TResult> WithClusterClient<TResult>(Func<ICommonsClusterClient, Task<TResult>> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            using (var client = builder.Build())
            {
                await client.Connect(GetRetryFilter(c));
                var logger = _serviceProvider.GetService<ILogger<Client>>();
                var cl = new Client(client, logger);
                try
                {
                    var res = await action(cl);
                    return res;
                }
                finally 
                {
                    await cl.Close(); 
                }
            }
        }

        public async Task<ICommonsClusterClient> GetUnmanagedClient()
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            var client = builder.Build();
            await client.Connect(GetRetryFilter(c));
            var logger = _serviceProvider.GetService<ILogger<Client>>();
            var cl = new Client(client, logger);
            return cl;
        }

        public async Task<ICommonsClusterClient> WithUnmanagedClient(Func<ICommonsClusterClient, Task> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            var client = builder.Build();
            await client.Connect(GetRetryFilter(c));
            var logger = _serviceProvider.GetService<ILogger<Client>>();
            var cl = new Client(client, logger);
            await action(cl);
            return cl;
        }

        public async Task<(ICommonsClusterClient, TResult)> WithUnmanagedClient<TResult>(Func<ICommonsClusterClient, Task<TResult>> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            var client = builder.Build();
            await client.Connect(GetRetryFilter(c));
            var logger = _serviceProvider.GetService<ILogger<Client>>();
            var cl = new Client(client, logger);
            return (cl, await action(cl));
        }

        private Func<Exception, Task<bool>> GetRetryFilter(Counter c)
        {
            return async (Exception exception) =>
            {
                _serviceProvider.GetService<ILogger<ClientFactory>>()?.LogWarning(
                    exception,
                    "Exception while attempting to connect to Orleans cluster"
                );
                if (c.Value <= 0)
                {
                    return false;
                }
                await Task.Delay(TimeSpan.FromSeconds(2));
                c.Value--;
                return true;
            };
        }

        private class Counter
        {
            public int Value { get; set; } = 0;
        }

    }
}
