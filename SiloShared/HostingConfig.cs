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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using CommunAxiom.Commons.Client.SiloShared.System;
using CommunAxiom.Commons.Orleans;

namespace CommunAxiom.Commons.Client.SiloShared
{
    public static class HostingConfig
    {

        public static IServiceCollection SetServerServices(this IServiceCollection sc)
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
        public static ISiloHostBuilder SetDefaults(this ISiloHostBuilder siloHostBuilder, out IConfiguration configuration)
        {
            siloHostBuilder.SetConfiguration(out configuration);
            siloHostBuilder.SetClustering();
            siloHostBuilder.SetEndPoints();
            siloHostBuilder.SetStreamProviders();
            siloHostBuilder.SetClusterOptions();
            return siloHostBuilder;
        }
        /// <summary>
        /// This is the silo clustering mode
        /// </summary>
        /// <param name="siloHostBuilder"></param>
        /// <returns></returns>
        public static ISiloHostBuilder SetClustering(this ISiloHostBuilder siloHostBuilder)
        {
            //TODO: Add support to multisilo cluster
            return siloHostBuilder.UseLocalhostClustering();
        }

        /// <summary>
        /// Configure endpoints ports and ig
        /// </summary>
        /// <param name="siloHostBuilder"></param>
        /// <returns></returns>
        public static ISiloHostBuilder SetEndPoints(this ISiloHostBuilder siloHostBuilder)
        {
            // the silo port was modified from the default because the option range for that port falls in an unauthorized range
            siloHostBuilder.ConfigureEndpoints(siloPort: 7717, gatewayPort: 30000);
            // TODO: use configuration to set the IP Address
            siloHostBuilder.Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);
            return siloHostBuilder;
        }

        /// <summary>
        /// Configure supported streams
        /// </summary>
        /// <param name="siloHostBuilder"></param>
        /// <returns></returns>
        public static ISiloHostBuilder SetStreamProviders(this ISiloHostBuilder siloHostBuilder)
        {
            siloHostBuilder.AddSimpleMessageStreamProvider(Constants.DefaultStream);
            return siloHostBuilder;
        }

        public static ISiloHostBuilder SetClusterOptions(this ISiloHostBuilder siloHostBuilder)
        {
            siloHostBuilder.Configure<ClusterOptions>(options =>
             {
                 options.ClusterId = "0.0.1-a1";
                 options.ServiceId = "CommonsClientCluster";
             })
            .ConfigureLogging(logging => logging.AddConsole());
            return siloHostBuilder;
        }

        public static IServiceCollection SetStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.SetJSONLiteDbSerializationProvider();
            services.Configure<LiteDbConfig>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configuration.GetSection("LiteDbStorage"));
            services.Configure<LiteDbConfig>(Constants.Storage.PubSubStore, configuration.GetSection(Constants.Storage.PubSubStore));
            services.Configure<LiteDbConfig>(Constants.Storage.JObjectStore, configuration.GetSection(Constants.Storage.JObjectStore));

            services.AddSingletonNamedService<IOptionsMonitor<LiteDbConfig>>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, (svc, key) =>
            {
                return svc.GetService<IOptionsMonitor<LiteDbConfig>>();
            });

            services.AddSingletonNamedService<IOptionsMonitor<LiteDbConfig>>(Constants.Storage.PubSubStore, (svc, key) =>
            {
                return svc.GetService<IOptionsMonitor<LiteDbConfig>>();
            });

            services.AddLiteDbGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
            services.AddWrappedLiteDbGrainStorage(Constants.Storage.PubSubStore);
            services.AddJObjectLiteDbGrainStorage(Constants.Storage.JObjectStore);

            return services;
        }

        public static ISiloHostBuilder SetConfiguration(this ISiloHostBuilder siloHostBuilder, out IConfiguration configuration)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./config.json");
            configuration = configurationBuilder.Build();

            return siloHostBuilder.ConfigureAppConfiguration(app =>
            {
                app.AddJsonFile("./config.json");
            });
        }

        public static IHostBuilder SetConfiguration(this IHostBuilder siloHostBuilder, out IConfiguration configuration)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./config.json");
            configuration = configurationBuilder.Build();

            return siloHostBuilder.ConfigureAppConfiguration(app =>
            {
                app.AddJsonFile("./config.json");
            });
        }

    }
}
