using Comax.Commons.Orchestrator;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
        public static IConfiguration Configuration { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            SetupTests();
            await MainSilo.StartSilo();
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {
            await MainSilo.StopSilo();
        }

        public static void SetupTests()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./config.json");
            Configuration = configurationBuilder.Build();

            ServiceCollection sc = new ServiceCollection();
            sc.AddSingleton<IConfiguration>(Configuration);
            sc.AddLogging(lb => lb.AddConsole());
            ServiceProvider = sc.BuildServiceProvider();
        }
    }
}
