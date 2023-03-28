using Comax.Commons.StorageProvider.Hosting;
using CommunAxiom.Commons.Client.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Runtime;
using System.Net;
using Microsoft.Extensions.Hosting;
using CommunAxiom.Commons.Client.SiloShared.System;
using CommunAxiom.Commons.Ingestion.Extentions;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Client.Silo;
using System;
using System.Collections.Generic;
using CommunAxiom.Commons.Client.AgentService.OrchClient;
using Comax.Commons.StorageProvider;

namespace CommunAxiom.Commons.Client.SiloShared
{
    public static class HostingConfig
    {
        public static IServiceCollection SetServerServices(this IServiceCollection sc)
        {
            sc.AddHostedService<HeartbeatService>();
            sc.AddIngestion();
            return sc;
        }

        /// <summary>
        /// This is what's common to any silo initialisation 
        /// </summary>
        /// <param name="siloHostBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ISiloHostBuilder SetDefaults(this ISiloHostBuilder siloHostBuilder, out IConfiguration configuration, string configFile)
        {
            siloHostBuilder.SetConfiguration(out configuration, configFile);
            siloHostBuilder.SetClustering(configuration);
            siloHostBuilder.SetEndPoints(configuration);
            siloHostBuilder.SetStreamProviders();
            siloHostBuilder.SetClusterOptions(configuration);
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
            siloHostBuilder.ConfigureEndpoints(siloPort: int.Parse(configuration["siloPort"].ToString()), gatewayPort: int.Parse(configuration["gatewayPort"].ToString()));

            if (configuration["advertisedIp"].StartsWith("127.0.0.1"))
            {
                // TODO: use configuration to set the IP Address
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

        public static ISiloHostBuilder SetClusterOptions(this ISiloHostBuilder siloHostBuilder, IConfiguration configuration)
        {
            siloHostBuilder.Configure<ClusterOptions>(options =>
             {
                 options.ClusterId = configuration["ClusterId"];
                 options.ServiceId = configuration["ServiceId"];
             })
            .ConfigureLogging(logging => logging.AddConsole());
            return siloHostBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection SetStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.SetJSONSerializationProvider();

            if(configuration["storage"] == "litedb")
            {
                services.Configure<LiteDbConfig>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configuration.GetSection("LiteDbStorage"));
                services.Configure<LiteDbConfig>(OrleansConstants.Storage.PubSubStore, configuration.GetSection(OrleansConstants.Storage.PubSubStore));
                services.Configure<LiteDbConfig>(OrleansConstants.Storage.JObjectStore, configuration.GetSection(OrleansConstants.Storage.JObjectStore));

                services.AddSingletonNamedService<IOptionsMonitor<LiteDbConfig>>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, (svc, key) =>
                {
                    return svc.GetService<IOptionsMonitor<LiteDbConfig>>();
                });

                services.AddSingletonNamedService<IOptionsMonitor<LiteDbConfig>>(OrleansConstants.Storage.PubSubStore, (svc, key) =>
                {
                    return svc.GetService<IOptionsMonitor<LiteDbConfig>>();
                });

                services.AddLiteDbGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
                services.AddWrappedLiteDbGrainStorage(OrleansConstants.Storage.PubSubStore);
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

        public static ISiloHostBuilder SetConfiguration(this ISiloHostBuilder siloHostBuilder, out IConfiguration configuration, string configFile)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(DefaultConfigs.Configs);
            configurationBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string,string>("ConfigPath", configFile ?? "./config.json"),
                new KeyValuePair<string,string>("advertisedIp", "127.0.0.1")
            });
            configurationBuilder.AddJsonFile(configFile ?? "./config.json");
            configurationBuilder.AddEnvironmentVariables();
            
            configuration = configurationBuilder.Build();

            return siloHostBuilder.ConfigureAppConfiguration(app =>
            {
                app.AddInMemoryCollection(DefaultConfigs.Configs);
                app.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string,string>("ConfigPath", configFile ?? "./config.json"),
                    new KeyValuePair<string,string>("advertisedIp", "127.0.0.1")
                });
                app.AddJsonFile(configFile ?? "./config.json");
                app.AddEnvironmentVariables();
                
            });
        }

        public static IHostBuilder SetConfiguration(this IHostBuilder siloHostBuilder, out IConfiguration configuration, string configFile)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(DefaultConfigs.Configs);
            configurationBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string,string>("ConfigPath", configFile ?? "./config.json"),
                new KeyValuePair<string,string>("advertisedIp", "127.0.0.1")
            });
            configurationBuilder.AddJsonFile(configFile ?? "./config.json");
            configurationBuilder.AddEnvironmentVariables();
            
            configuration = configurationBuilder.Build();

            return siloHostBuilder.ConfigureAppConfiguration(app =>
            {
                app.AddInMemoryCollection(DefaultConfigs.Configs);
                app.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string,string>("ConfigPath", configFile ?? "./config.json"),
                    new KeyValuePair<string,string>("advertisedIp", "127.0.0.1")
                });
                app.AddJsonFile(configFile ?? "./config.json");
                app.AddEnvironmentVariables();
            });
        }

    }
}
