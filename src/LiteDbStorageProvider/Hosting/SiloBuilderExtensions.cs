using Comax.Commons.StorageProvider.Serialization;
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
    public static class SiloBuilderExtensions
    {
        public static IServiceCollection AddLiteDbGrainStorage(this IServiceCollection services, string name,
            Action<OptionsBuilder<LiteDbConfig>> configureOptions = null)
        {
            services.AddTransient<IConfigurationValidator>(sp => 
                new LiteDbConfigValidator(name, sp.GetRequiredService<IOptionsMonitor<LiteDbConfig>>().Get(name)));
            services.ConfigureNamedOptionForLogging<LiteDbConfig>(name);

            services.TryAddSingleton<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddSingletonNamedService<IGrainStorage>(name, (s,n)=>
                                        new DefaultStorageProvider(n, 
                                                            s.GetRequiredService<ILogger<DefaultStorageProvider>>(),
                                                            s.GetRequiredService<IOptionsMonitor<LiteDbConfig>>().Get(name),
                                                            s))
                           .AddSingletonNamedService(name, (s, n) =>
                                (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }

        public static IServiceCollection AddWrappedLiteDbGrainStorage(this IServiceCollection services, string name,
            Action<OptionsBuilder<LiteDbConfig>> configureOptions = null)
        {
            services.AddTransient<IConfigurationValidator>(sp =>
                new LiteDbConfigValidator(name, sp.GetRequiredService<IOptionsMonitor<LiteDbConfig>>().Get(name)));
            services.ConfigureNamedOptionForLogging<LiteDbConfig>(name);

            services.TryAddSingleton<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddSingletonNamedService<IGrainStorage>(name, (s, n) =>
                                        new WrappedLiteDbStorageProvider(n,
                                                            s.GetRequiredService<ILogger<WrappedLiteDbStorageProvider>>(),
                                                            s.GetRequiredService<IOptionsMonitor<LiteDbConfig>>().Get(name),
                                                            s))
                           .AddSingletonNamedService(name, (s, n) =>
                                (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }

        public static IServiceCollection AddJObjectLiteDbGrainStorage(this IServiceCollection services, string name,
            Action<OptionsBuilder<LiteDbConfig>> configureOptions = null)
        {
            services.AddTransient<IConfigurationValidator>(sp =>
                new LiteDbConfigValidator(name, sp.GetRequiredService<IOptionsMonitor<LiteDbConfig>>().Get(name)));
            services.ConfigureNamedOptionForLogging<LiteDbConfig>(name);

            return services.AddSingletonNamedService<IGrainStorage>(name, (s, n) =>
                                        new JObjectStorageProvider(n,
                                                            s.GetRequiredService<ILogger<JObjectStorageProvider>>(),
                                                            s.GetRequiredService<IOptionsMonitor<LiteDbConfig>>().Get(name),
                                                            s))
                           .AddSingletonNamedService(name, (s, n) =>
                                (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }

        
    }
}
