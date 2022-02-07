using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            configureOptions?.Invoke(services.AddOptions<LiteDbConfig>(name));
            services.AddTransient<IConfigurationValidator>(sp => new LiteDbConfigValidator(name, sp.GetRequiredService<IOptionsMonitor<LiteDbConfig>>().Get(name)));
            services.ConfigureNamedOptionForLogging<LiteDbConfig>(name);

            services.TryAddSingleton<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddSingletonNamedService<IGrainStorage, LiteDbStorageProvider>(name)
                           .AddSingletonNamedService(name, (s, n) =>
                                (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));

        }
    }
}
