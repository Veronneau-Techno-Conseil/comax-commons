using Comax.Commons.Orchestrator;
using Comax.Commons.CommonsShared.ApiMembershipProvider;
using Comax.Commons.Orchestrator.Client;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using FluentAssertions.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Orleans;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests
{
    [SetUpFixture]
    public class Cluster
    {
        public static bool NoAuth { get; set; }

        public static bool AsCommonsAgent { get; set; }

        public static IConfiguration Configuration { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }

        

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            SetupTests();
            if (Configuration["client_mode"] == "local")
                await MainSilo.StartSilo();
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {
            if (Configuration["client_mode"] == "local")
                await MainSilo.StopSilo();
        }

        public static void SetupTests()
        {
            if (!Directory.Exists("./dbs"))
            {
                Directory.CreateDirectory("./dbs");
            }
            var files = System.IO.Directory.GetFiles("./dbs");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./config.json");
            configurationBuilder.AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();

            ServiceCollection sc = new ServiceCollection();
            sc.AddSingleton<IConfiguration>(Configuration);
            sc.AddLogging(lb => lb.AddConsole());


            /*
             * Client specific
             * */

            sc.AddSingleton<IOrchestratorClientConfig, ClientConfig>();
            sc.AddSingleton<IOutgoingGrainCallFilter>(sp =>
            {
                var logger = sp.GetService<ILogger<SecureTokenOutgoingFilter>>();
                var oidc = new OIDCSettings();
                Configuration.Bind("ClientOIDC", oidc);
                return new SecureTokenOutgoingFilter(logger, new TestTokenProvider(oidc, Configuration));
            });

            /*
             * End client specific
             * */

            ServiceProvider = sc.BuildServiceProvider();
        }

        

        public class ClientConfig : IOrchestratorClientConfig
        {
            public void Configure(IServiceCollection sc)
            {
                sc.AddLogging(l=>l.AddConsole());
                sc.AddSingleton<SecureTokenOutgoingFilter>(sp =>
                {
                    var logger = sp.GetService<ILogger<SecureTokenOutgoingFilter>>();
                    var oidc = new OIDCSettings();
                    Configuration.Bind("ClientOIDC", oidc);
                    return new SecureTokenOutgoingFilter(logger, new TestTokenProvider(oidc, Configuration));
                });

                sc.AddSingleton<ISettingsProvider>(x=> new ConfigSettingsProvider("ClientOIDC", Configuration));
                sc.AddSingleton<ISvcClientFactory, SvcClientFactory>();
                //sc.AddApiProvider(c => Configuration.GetSection("membership").Bind(c));
            }
        }
    }
}
