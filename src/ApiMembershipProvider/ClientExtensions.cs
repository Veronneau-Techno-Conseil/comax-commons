using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Messaging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.ApiMembershipProvider
{
    public static class MongoDBClientExtensions
    {
        
        /// <summary>
        /// Configure client to use MongoGatewayListProvider
        /// </summary>
        public static IClientBuilder UseMongoDBClustering(this IClientBuilder builder,
            Action<MongoDBOptions> configurator = null)
        {
            return builder.ConfigureServices(services => services.AddMongoDBGatewayListProvider(configurator));
        }

        /// <summary>
        /// Configure client to use MongoGatewayListProvider
        /// </summary>
        public static IClientBuilder UseMongoDBClustering(this IClientBuilder builder,
            IConfiguration configuration)
        {
            return builder.ConfigureServices(services => services.AddMongoDBGatewayListProvider(configuration));
        }

        /// <summary>
        /// Configure silo to use MongoGatewayListProvider.
        /// </summary
        public static IServiceCollection AddMongoDBGatewayListProvider(this IServiceCollection services,
            Action<MongoDBOptions> configurator = null)
        {
            services.Configure(configurator ?? (x => { }));
            services.AddSingleton<IGatewayListProvider, MongoGatewayListProvider>();

            return services;
        }

        /// <summary>
        /// Configure silo to use MongoGatewayListProvider.
        /// </summary>
        public static IServiceCollection AddMongoDBGatewayListProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MongoDBOptions>(configuration);
            services.AddSingleton<IGatewayListProvider, MongoGatewayListProvider>();

            return services;
        }
    }
}
