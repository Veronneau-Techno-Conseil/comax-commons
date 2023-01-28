
using CommunAxiom.Commons.Shared.OIDC;
using FluentAssertions.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Orleans;

namespace Shared.Tests
{
    [SetUpFixture]
    public class Setup
    {
        public static IConfiguration Configuration { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }

        

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            SetupTests();
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {

        }

        public static void SetupTests()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./config.json");
            configurationBuilder.AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();

            ServiceCollection sc = new ServiceCollection();
            sc.AddSingleton<IConfiguration>(Configuration);
            sc.AddLogging(lb => lb.AddConsole());

            ServiceProvider = sc.BuildServiceProvider();
        }
    }
}
