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
        public static IClientBuilder UseApiClustering(this IClientBuilder builder,
            Action<ApiMembershipClientConfig> configurator = null)
        {
            return builder.ConfigureServices(services => services.AddApiProvider(configurator));
        }

        /// <summary>
        /// Configure client to use MongoGatewayListProvider
        /// </summary>
        public static IClientBuilder UseMongoDBClustering(this IClientBuilder builder,
            IConfiguration configuration)
        {
            return builder.ConfigureServices(services => services.AddApiProvider(configuration));
        }

        public static IServiceCollection AddApiProvider(this IServiceCollection services,
            Action<ApiMembershipClientConfig> configurator = null)
        {
            services.Configure(configurator ?? (x => { }));
            services.AddSingleton<IGatewayListProvider, ApiGatewayListProvider>();

            return services;
        }

        public static IServiceCollection AddApiProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiMembershipClientConfig>(configuration);
            services.AddSingleton<IGatewayListProvider, ApiGatewayListProvider>();

            return services;
        }
    }
}
