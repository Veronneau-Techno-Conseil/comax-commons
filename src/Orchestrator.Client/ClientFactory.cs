﻿using Comax.Commons.Orchestrator.Contracts;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Comax.Commons.Orchestrator.ApiMembershipProvider;

namespace Comax.Commons.Orchestrator.Client
{
    public class ClientFactory : IOrchestratorClientFactory
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
                .ConfigureAppConfiguration((ctxt, cb)=> {
                    cb.AddConfiguration(configuration);
                })
                .ConfigureServices(services =>
                {
                    var conf = serviceProvider.GetService<IOrchestratorClientConfig>();
                    if (conf != null)
                        conf.Configure(services);
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "0.0.1-a1";
                    options.ServiceId = "OrchestratorCluster";
                })
                .ConfigureApplicationParts(parts =>
                {
                    parts.AddFromApplicationBaseDirectory();
                });

            if (configuration["client_mode"] == "local")
            {
                b.UseLocalhostClustering(30001);
            }
            else
            {
                b.UseApiClustering(mo =>
                {
                    configuration.GetSection("membership").Bind(mo);
                });
            }
                
            b.AddOutgoingGrainCallFilter(serviceProvider.GetService<SecureTokenOutgoingFilter>())
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

        public async Task WithClusterClient(Func<IOrchestratorClient, Task> action)
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
        public async Task<TResult> WithClusterClient<TResult>(Func<IOrchestratorClient, Task<TResult>> action)
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

        public async Task<IOrchestratorClient> WithUnmanagedClient(Func<IOrchestratorClient, Task> action)
        {
            Counter c = new Counter();
            var builder = GetBuilder();
            var client = builder.Build();
            await client.Connect(GetRetryFilter(c));
            var cl = new Client(client);
            await action(cl);
            return cl;
        }

        public async Task<(IOrchestratorClient, TResult)> WithUnmanagedClient<TResult>(Func<IOrchestratorClient, Task<TResult>> action)
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
