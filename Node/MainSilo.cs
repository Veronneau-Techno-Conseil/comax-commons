
using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Comax.Commons.Shared.OIDC;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using Comax.Commons.Orchestrator.Contracts.UriRegistry;
using Comax.Commons.Orchestrator.UriRegistryGrain;
using Comax.Commons.Orchestrator.EventMailboxGrain;
using Comax.Commons.Orchestrator.Contracts.Central;
using Comax.Commons.Orchestrator.CentralGrain;
using System.Net.Http;
using Comax.Commons.Orchestrator.MailboxGrain;
using Comax.Commons.Orchestrator.Contracts.SOI;
using Comax.Commons.Orchestrator.SOIGrain;
using Comax.Commons.Orchestrator.MailGrain;

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
                    services.SetStorage(conf);

                    services.CentralGrainSetup();

                    //register singleton services for each grain/interface
                    //services.AddSingleton<ISubjectOfInterest, SubjectOfInterest>();
                    services.AddSingleton<IPublicBoard, PublicBoard>();
                    services.AddSingleton<ICentral, Central>();

                    services.AddSingleton<ISettingsProvider, OrchestratorSettingsProvider>();
                    services.AddSingleton<IClaimsPrincipalProvider, OIDCClaimsProvider>();
                    services.AddSingleton<IIncomingGrainCallFilter, AccessControlFilter>();
                    services.AddSingleton<ITokenProvider, ClientTokenProvider>();
                    services.AddTransient<HttpClient>();

                    //registering singleton service for UriRegistry grain
                    services.AddSingleton<IUriRegistry, UriRegistry>();

                    services.AddSingleton<IEventMailbox, EventMailbox>();

                })
                //configure application parts for each grain
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PublicBoard).Assembly).WithReferences())
                .ConfigureApplicationParts(parts=>parts.AddApplicationPart(typeof(UriRegistry).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(EventMailbox).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Mail).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(SubjectOfInterest).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Central).Assembly).WithReferences());

            var silo = builder.Build();
            await silo.StartAsync();
            _silo = silo;
            IsSiloStarted = true;
            _ = _silo.Stopped.ContinueWith(t => IsSiloStarted = false);
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
