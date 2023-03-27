using Comax.Commons.StorageProvider.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider
{
    public static class SiloBuilderExtensions
    {
        public static IServiceCollection SetStdSerializationProvider(this IServiceCollection services)
        {
            services.AddSingletonNamedService<ISerializationProvider, StdSerializationProvider>("standard");
            return services;
        }
        public static IServiceCollection SetJSONSerializationProvider(this IServiceCollection services)
        {
            services.AddSingletonNamedService<ISerializationProvider, NewtonsoftSerializationProvider>("standard");
            return services;
        }
    }
}
