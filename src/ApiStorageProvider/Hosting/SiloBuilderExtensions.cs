using Comax.Commons.ApiStorageProvider;
using Comax.Commons.StorageProvider.Serialization;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Hosting
{
    public static class SiloBuilderApiStorageExtensions
    {
        public static IServiceCollection AddApiGrainStorage(this IServiceCollection services, string name, string configKey = null)
        {
            services.AddSingleton<TokenManager>();
            services.AddSingleton<GrainStorageClientFactory>(sp =>
            {
                var tm = sp.GetService<TokenManager>();
                var opts = sp.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name);
                var store = new GrainStorageClientFactory(tm, opts);
                return store;
            });
            services.AddTransient<IConfigurationValidator>(sp => 
                new ApiStorageConfigValidator(name, sp.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name)));
            services.ConfigureNamedOptionForLogging<ApiStorageConfiguration>(name);

            services.TryAddTransient<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddTransientNamedService<IGrainStorage>(name, (s,n)=>
                                        new DefaultStorageProvider(n, 
                                                            s.GetRequiredService<ILogger<DefaultStorageProvider>>(),
                                                            s.GetRequiredService<GrainStorageClientFactory>(),
                                                            s.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name)))
                           .AddTransientNamedService(name, (s, n) =>
                                (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }

        public static IServiceCollection AddWrappedApiGrainStorage(this IServiceCollection services, string name, string configKey = null)
        {
            services.AddSingleton<TokenManager>();
            services.AddSingleton<GrainStorageClientFactory>(sp =>
            {
                var tm = sp.GetService<TokenManager>();
                var opts = sp.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name);
                var store = new GrainStorageClientFactory(tm, opts);
                return store;
            });
            services.AddTransient<IConfigurationValidator>(sp =>
                new ApiStorageConfigValidator(name, sp.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name)));
            services.ConfigureNamedOptionForLogging<ApiStorageConfiguration>(name);

            services.TryAddTransient<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddTransientNamedService<IGrainStorage>(name, (s, n) =>
                                        new WrappedLiteDbStorageProvider(n,
                                                            s.GetRequiredService<ILogger<WrappedLiteDbStorageProvider>>(),
                                                            s.GetRequiredService<GrainStorageClientFactory>(),
                                                            s,
                                                            s.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name)))
                           .AddTransientNamedService(name, (s, n) =>
                                (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }

        public static IServiceCollection AddJObjectApiGrainStorage(this IServiceCollection services, string name, string configKey = null)
        {
            services.AddSingleton<TokenManager>();
            services.AddSingleton<GrainStorageClientFactory>(sp =>
            {
                var tm = sp.GetService<TokenManager>();
                var opts = sp.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name);
                var store = new GrainStorageClientFactory(tm, opts);
                return store;
            });
            services.AddTransient<IConfigurationValidator>(sp =>
                new ApiStorageConfigValidator(name, sp.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name)));
            services.ConfigureNamedOptionForLogging<ApiStorageConfiguration>(name);

            return services.AddTransientNamedService<IGrainStorage>(name, (s, n) =>
                                        new JObjectStorageProvider(n,
                                                            s.GetRequiredService<ILogger<JObjectStorageProvider>>(),
                                                            s.GetRequiredService<GrainStorageClientFactory>(),
                                                            s.GetRequiredService<IOptionsMonitor<ApiStorageConfiguration>>().Get(configKey ?? name)))
                           .AddTransientNamedService(name, (s, n) =>
                                (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }

    }
}
