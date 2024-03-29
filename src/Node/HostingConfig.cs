﻿using Comax.Commons.StorageProvider.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Comax.Commons.Orchestrator.Contracts;
using CommunAxiom.Commons.Orleans;
using static IdentityModel.ClaimComparer;
using MongoDB.Driver;
using Newtonsoft.Json;
using Comax.Commons.StorageProvider;

namespace Comax.Commons.Orchestrator
{
    public static class HostingConfig
    {
        const string PubSubStore = "PubSubStore";

        /// <summary>
        /// Kickstarts and maintains the silo
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public static IServiceCollection SetServerServices(this IServiceCollection sc, IConfiguration configuration)
        {
            sc.AddHostedService<HeartbeatService>();

            return sc;
        }

        /// <summary>
        /// This is what's common to any silo initialisation 
        /// </summary>
        /// <param name="siloHostBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ISiloHostBuilder SetDefaults(this ISiloHostBuilder siloHostBuilder, Action<IServiceCollection, IConfiguration> configureServices, string configurationFile)
        {
            siloHostBuilder.ConfigureLogging(opts =>
            {
                opts.AddConsole();
                opts.SetMinimumLevel(LogLevel.Debug);
            });
            IConfiguration configuration = null;
            siloHostBuilder.SetConfiguration(out configuration, configurationFile);
            siloHostBuilder.ConfigureServices(sc => configureServices(sc, configuration));
            siloHostBuilder.SetClustering(configuration);
            siloHostBuilder.SetEndPoints(configuration);
            siloHostBuilder.SetStreamProviders();
            siloHostBuilder.SetClusterOptions();
            return siloHostBuilder;
        }
        /// <summary>
        /// This is the silo clustering mode
        /// </summary>
        /// <param name="siloHostBuilder"></param>
        /// <returns></returns>
        public static ISiloHostBuilder SetClustering(this ISiloHostBuilder siloHostBuilder, IConfiguration configuration)
        {
            if (configuration["advertisedIp"].StartsWith("127.0.0.1"))
            {
                return siloHostBuilder.UseLocalhostClustering();
            }
            return siloHostBuilder;
        }

        /// <summary>
        /// Configure endpoints ports and ig
        /// </summary>
        /// <param name="siloHostBuilder"></param>
        /// <returns></returns>
        public static ISiloHostBuilder SetEndPoints(this ISiloHostBuilder siloHostBuilder, IConfiguration configuration)
        {
            // the silo port was modified from the default because the option range for that port falls in an unauthorized range

            siloHostBuilder.ConfigureEndpoints(siloPort: int.Parse(configuration["siloPort"]), gatewayPort: int.Parse(configuration["gatewayPort"]));
            
            if (configuration["advertisedIp"].StartsWith("127.0.0.1"))
            {
                siloHostBuilder.Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);
                siloHostBuilder.UseLocalhostClustering(int.Parse(configuration["siloPort"]), int.Parse(configuration["gatewayPort"]));
            }
            else
            {
                siloHostBuilder.Configure<EndpointOptions>(options => 
                {
                    options.AdvertisedIPAddress = IPAddress.Parse(configuration["advertisedIp"]);
                    options.SiloListeningEndpoint = IPEndPoint.Parse($"{configuration["listeningEndpoint"]}:{configuration["siloPort"]}");
                    options.GatewayListeningEndpoint = IPEndPoint.Parse($"{configuration["listeningEndpoint"]}:{configuration["gatewayPort"]}");
                    Console.WriteLine($"STARTUP: Advertizing: {configuration["advertisedIp"]}, SiloEndpoint: {configuration["listeningEndpoint"]}:{configuration["siloPort"]}, GatewayEndpoint: {configuration["listeningEndpoint"]}:{configuration["gatewayPort"]}");
                });
            }
           
            return siloHostBuilder;
        }

        /// <summary>
        /// Configure supported streams
        /// </summary>
        /// <param name="siloHostBuilder"></param>
        /// <returns></returns>
        public static ISiloHostBuilder SetStreamProviders(this ISiloHostBuilder siloHostBuilder)
        {
            siloHostBuilder.AddSimpleMessageStreamProvider(OrleansConstants.Streams.DefaultStream);
            siloHostBuilder.AddSimpleMessageStreamProvider(OrleansConstants.Streams.ImplicitStream, opts =>
            {
                opts.FireAndForgetDelivery = true;
            });

            return siloHostBuilder;
        }

        public static ISiloHostBuilder SetClusterOptions(this ISiloHostBuilder siloHostBuilder)
        {
            siloHostBuilder.Configure<ClusterOptions>(options =>
             {
                 options.ClusterId = "0.0.1-a1";
                 options.ServiceId = "OrchestratorCluster";
             })
            .ConfigureLogging(logging => logging.AddConsole());
            return siloHostBuilder;
        }

        public static IServiceCollection SetStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.SetJSONSerializationProvider();

            if (configuration["storage"] == "litedb")
            {
                
                services.Configure<LiteDbConfig>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configuration.GetSection("LiteDbStorage"));
                services.Configure<LiteDbConfig>(PubSubStore, configuration.GetSection(PubSubStore));

                services.AddSingletonNamedService<IOptionsMonitor<LiteDbConfig>>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, (svc, key) =>
                {
                    return svc.GetService<IOptionsMonitor<LiteDbConfig>>();
                });

                services.AddSingletonNamedService<IOptionsMonitor<LiteDbConfig>>(PubSubStore, (svc, key) =>
                {
                    return svc.GetService<IOptionsMonitor<LiteDbConfig>>();
                });

                services.AddLiteDbGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
                services.AddWrappedLiteDbGrainStorage(PubSubStore);
                services.AddJObjectLiteDbGrainStorage(OrleansConstants.Storage.JObjectStore);

            }
            else
            {
                services.Configure<ApiStorageConfiguration>("ApiStorage", configuration.GetSection("ApiStorage"));
                services.AddSingletonNamedService<IOptionsMonitor<ApiStorageConfiguration>>("ApiStorage", (svc, key) =>
                {
                    return svc.GetService<IOptionsMonitor<ApiStorageConfiguration>>();
                });

                services.AddApiGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, "ApiStorage");
                services.AddJObjectApiGrainStorage(OrleansConstants.Storage.JObjectStore, "ApiStorage");
                services.AddWrappedApiGrainStorage(OrleansConstants.Storage.PubSubStore, "ApiStorage");

            }
            

            return services;
        }

        public static ISiloHostBuilder SetConfiguration(this ISiloHostBuilder siloHostBuilder, out IConfiguration configuration, string configurationFile)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(configurationFile ?? "./config.json");
            configurationBuilder.AddEnvironmentVariables();
            configuration = configurationBuilder.Build();

            return siloHostBuilder.ConfigureAppConfiguration(app =>
            {
                app.AddJsonFile("./config.json");
                app.AddEnvironmentVariables();
            });
        }

        public static IHostBuilder SetConfiguration(this IHostBuilder siloHostBuilder, out IConfiguration configuration)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./config.json");
            configurationBuilder.AddEnvironmentVariables();
            configuration = configurationBuilder.Build();

            return siloHostBuilder.ConfigureAppConfiguration(app =>
            {
                app.AddJsonFile("./config.json");
                app.AddEnvironmentVariables();
            });
        }
    }
}
