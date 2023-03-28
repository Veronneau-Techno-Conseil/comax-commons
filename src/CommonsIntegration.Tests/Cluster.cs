using Comax.Commons.Orchestrator;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.FlowControl;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.DotnetSdk.Helpers;
using CommunAxiom.DotnetSdk.Helpers.OIDC;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Orleans;

namespace CommonsIntegration.Tests
{
    [SetUpFixture]
    public class Cluster
    {
        private static IServiceProvider ServiceProvider { get; set; }

        private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public static Orchestrator OrchestratorInstance { get; set; }

        public class Orchestrator
        {
            public IConfiguration Configuration { get; set; }
            public IServiceProvider ServiceProvider { get; set; }
            public SegregatedContext<Orchestrator> Context { get; set; }
            public IHost StorageApiService { get; set; }
        }

        public static Commons CommonsInstance1 { get; set; }
        public static Commons CommonsInstance2 { get; set; }

        public class Commons
        {
            public CommunAxiom.Commons.Client.Silo.MainSilo Silo { get; set; }
            public AgentReferee.RefereeApp AgentReferee { get; set; }            
            public SegregatedContext<Commons> Context { get; set; }
        }


        public static Client ClientInstance1 { get; set; }
        public static Client ClientInstance2 { get; set; }
        public class Client
        {
            //TODO create service provider with logging and IOutgoingGrainCallFilter the graincallfilter should set the outgoing token
            public IConfiguration Configuration { get; set; }
            public IServiceProvider ServiceProvider { get; set; }
            public CommunAxiom.Commons.Client.ClusterClient.ClientFactory ClientFactory { get; set; }
            public SegregatedContext<Client> Context { get; set; }

            public async Task WaitForConnection()
            {
                var cnt = 0;
                bool connected = false;
                connected = await this.ClientFactory.TestConnection();
                while (!connected && cnt < 10)
                {
                    await Task.Delay(5000);
                    connected = await this.ClientFactory.TestConnection();
                }
                connected.Should().BeTrue();
            }
        }

        public static bool NoAuth { get; set; }

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            Setup();

            //Orchestrator
            OrchestratorInstance = new Orchestrator();
            OrchestratorInstance.Context = new SegregatedContext<Orchestrator>(
                async () =>
                {
                    SetupOrchestratorTests();

                    OrchestratorInstance.StorageApiService = await GrainStorageService.Application.CreateApp("./grainstoresvc.config.json");
                    _ = OrchestratorInstance.StorageApiService.RunAsync(cancellationTokenSource.Token);
                   
                    return OrchestratorInstance;
                },
                cancellationTokenSource.Token,
                ServiceProvider.GetService<ILogger<SegregatedContext<Orchestrator>>>());
            await OrchestratorInstance.Context.WaitForContext();

            if (OrchestratorInstance.Configuration["client_mode"] == "local")
            {
                await OrchestratorInstance.Context.Run(async (conf) => await MainSilo.StartSilo("./orchestrator.config.json"));
            }

            //Commons Instance 1
            CommonsInstance1 = new Commons();
            CommonsInstance1.Context = new SegregatedContext<Commons>(
                async () =>
                {
                    CommonsInstance1.AgentReferee = new AgentReferee.RefereeApp("./commons1.config.json");
                    CommonsInstance1.AgentReferee.Init();
                    _ = CommonsInstance1.AgentReferee.RunAsync();
                    CommonsInstance1.Silo = new CommunAxiom.Commons.Client.Silo.MainSilo();
                    _ = CommonsInstance1.Silo.StartSilo("./commons1.config.json");
                    return CommonsInstance1;
                }, 
                cancellationTokenSource.Token,
                ServiceProvider.GetService<ILogger<SegregatedContext<Commons>>>()
            );
            await CommonsInstance1.Context.WaitForContext();

            //Commons Instance 2
            CommonsInstance2 = new Commons();
            CommonsInstance2.Context = new SegregatedContext<Commons>(
                async () =>
                {
                    CommonsInstance2.AgentReferee = new AgentReferee.RefereeApp("./commons2.config.json");
                    CommonsInstance2.AgentReferee.Init();
                    _ = CommonsInstance2.AgentReferee.RunAsync();
                    CommonsInstance2.Silo = new CommunAxiom.Commons.Client.Silo.MainSilo();
                    _ = CommonsInstance2.Silo.StartSilo("./commons2.config.json");
                    return CommonsInstance2;
                },
                cancellationTokenSource.Token,
                ServiceProvider.GetService<ILogger<SegregatedContext<Commons>>>()
            );
            await CommonsInstance2.Context.WaitForContext();

            //Commons Client 1
            ClientInstance1 = new Client();
            ClientInstance1.Context = new SegregatedContext<Client>(
                async () =>
                {
                    ConfigurationBuilder cb = new ConfigurationBuilder();
                    cb.AddJsonFile("./client1.config.json");
                    ClientInstance1.Configuration = cb.Build();

                    ServiceCollection sc = new ServiceCollection();
                    sc.AddLogging(x => x.AddConsole());
                    sc.AddSingleton<ISettingsProvider, ConfigSettingsProvider>(x => new ConfigSettingsProvider("OIDC", ClientInstance1.Configuration));
                    sc.AddSingleton<ITokenProvider, ClientOutgoingTokenProvider>();
                    sc.AddSingleton<IOutgoingGrainCallFilter, SecureTokenOutgoingFilter>();
                    sc.AddSingleton<IConfiguration>(ClientInstance1.Configuration);

                    ClientInstance1.ServiceProvider = sc.BuildServiceProvider();

                    ClientInstance1.ClientFactory = new CommunAxiom.Commons.Client.ClusterClient.ClientFactory(ClientInstance1.ServiceProvider, "./client1.config.json");
                    return ClientInstance1;
                },
                cancellationTokenSource.Token,
                ServiceProvider.GetService<ILogger<SegregatedContext<Client>>>()
            );
            await ClientInstance1.Context.WaitForContext();

            //Commons Client 2
            ClientInstance2 = new Client();
            ClientInstance2.Context = new SegregatedContext<Client>(
                async () =>
                {
                    ConfigurationBuilder cb = new ConfigurationBuilder();
                    cb.AddJsonFile("./client2.config.json");
                    ClientInstance2.Configuration = cb.Build();

                    ServiceCollection sc = new ServiceCollection();
                    sc.AddLogging(x => x.AddConsole());
                    sc.AddSingleton<ISettingsProvider, ConfigSettingsProvider>(x => new ConfigSettingsProvider("OIDC", ClientInstance2.Configuration));
                    sc.AddSingleton<ITokenProvider, ClientOutgoingTokenProvider>();
                    sc.AddSingleton<IOutgoingGrainCallFilter, SecureTokenOutgoingFilter>();
                    sc.AddSingleton<IConfiguration>(ClientInstance2.Configuration);

                    ClientInstance2.ServiceProvider = sc.BuildServiceProvider();
                    ClientInstance2.ClientFactory = new CommunAxiom.Commons.Client.ClusterClient.ClientFactory(ClientInstance2.ServiceProvider, "./client2.config.json");
                    return ClientInstance2;
                },
                cancellationTokenSource.Token,
                ServiceProvider.GetService<ILogger<SegregatedContext<Client>>>()
            );
            await ClientInstance2.Context.WaitForContext();

        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {
            await OrchestratorInstance.Context.Run(async c =>
            {
                if (OrchestratorInstance.Configuration["client_mode"] == "local")
                    await MainSilo.StopSilo();
            }); 
            try
            {
                //TODO: investigate why stop silo hangs without completing
                _ = CommonsInstance1.Context.Run(async c => await CommonsInstance1.Silo.StopSilo());
                _ = CommonsInstance2.Context.Run(async c => await CommonsInstance2.Silo.StopSilo());

                cancellationTokenSource.Cancel();
                OrchestratorInstance.Context.Dispose();
                CommonsInstance1.Context.Dispose();
                CommonsInstance2.Context.Dispose();
                ClientInstance1.Context.Dispose();
                ClientInstance2.Context.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void Setup()
        {
            ClearDbs();

            ServiceCollection services = new ServiceCollection();
            services.AddLogging(cfg => cfg.AddConsole());

            ServiceProvider = services.BuildServiceProvider();
        }

        public static void SetupOrchestratorTests()
        {
            //Orchestrator
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./orchestrator.config.json");
            configurationBuilder.AddEnvironmentVariables();
            OrchestratorInstance.Configuration = configurationBuilder.Build();

            ServiceCollection sc = new ServiceCollection();
            sc.AddSingleton<IConfiguration>(OrchestratorInstance.Configuration);
            sc.AddLogging(lb => lb.AddConsole());
            OrchestratorInstance.ServiceProvider = sc.BuildServiceProvider();
        }

        public static void ClearDbs()
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
        }

        public class ClientOutgoingTokenProvider : ITokenProvider
        {
            private readonly ISettingsProvider _settingsProvider;
            private readonly IConfiguration _configuration;
            private readonly TestUserConfig _testUserConfig;
            private string _token;

            public ClientOutgoingTokenProvider(ISettingsProvider settingsProvider, IConfiguration configuration) 
            { 
                _settingsProvider = settingsProvider;
                _configuration = configuration;
                _testUserConfig = new TestUserConfig();
                _configuration.GetSection("TestUser").Bind(_testUserConfig);
            }

            public async Task<string?> FetchToken()
            {
                if (string.IsNullOrEmpty(_token))
                {
                    var settings = await _settingsProvider.GetOIDCSettings();
                    var client = new TokenClient(settings);
                    var (res, tokenData) = await client.AuthenticatePassword(_testUserConfig.username, _testUserConfig.password);
                    res.Should().BeTrue();
                    _token = tokenData?.access_token;
                }
                return _token;
            }
        }

        public class TestUserConfig
        {
            public string username { get; set; }
            public string password { get; set; }
        }

    }
}
