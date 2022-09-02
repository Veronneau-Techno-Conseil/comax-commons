
using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Comax.Commons.Orchestrator.MailboxGrain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using Orleans.Security;
using Orleans.Security.Clustering;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.UriRegistry;
using Comax.Commons.Orchestrator.UriRegistryGrain;

namespace Comax.Commons.Orchestrator
{
    public static class MainSilo
    {
        static ISiloHost _silo;

        public static ISiloHost Host { get { return _silo; } }
        public static bool IsSiloStarted { get; private set; }

        public static async Task StartSilo()
        {
            if (IsSiloStarted)
            {
                return;
            }
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .SetDefaults(out var conf)
                .UseDashboard()
                .ConfigureServices(services =>
                {
                    ConfigureIdentitty(services, conf);

                    services.SetStorage(conf);

                    //register singleton services for each grain/interface
                    services.AddSingleton<IMailbox, Mailbox>();

                    //registering singleton service for UriRegistry grain
                    services.AddSingleton<IUriRegistry, UriRegistry>();

                })
                //configure application parts for each grain
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Mailbox).Assembly).WithReferences())
                .ConfigureApplicationParts(parts=>parts.AddApplicationPart(typeof(UriRegistry).Assembly).WithReferences());

            var silo = builder.Build();
            await silo.StartAsync();
            _silo = silo;
            IsSiloStarted = true;
            _ = _silo.Stopped.ContinueWith(t => IsSiloStarted = false);
        }

        static void ConfigureIdentitty(IServiceCollection services, IConfiguration configuration)
        {
            var configs = new Orleans.Security.IdentityServer4Info("https://accounts.communaxiom.org/", configuration["OIDC:ClientId"], configuration["OIDC:Secret"], "Orchestrator");

            services.AddOrleansClusteringAuthorization(configs,
                config =>
                {
                    config.ConfigureAuthorizationOptions = Security.ConfigurePolicyOptions;
                    config.TracingEnabled = true;
                });
        }

        public static async Task StopSilo()
        {
            if (IsSiloStarted)
            {
                await _silo.StopAsync();
                _silo.Dispose();
                _silo = null;
                IsSiloStarted = false;
            }
        }
    }
}
