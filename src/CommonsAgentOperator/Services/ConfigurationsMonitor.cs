using Comax.Central;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.DotnetSdk.Helpers.OIDC;
using k8s.Models;
using KubeOps.KubernetesClient;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Text;

namespace CommunAxiom.Commons.Client.Hosting.Operator.Services
{
    public class ConfigurationsMonitor : BackgroundService
    {
        private readonly ILogger<ConfigurationsMonitor> _logger;
        private OIDCSettings _oidcSettings;
        private readonly IConfiguration _configuration;
        private readonly IKubernetesClient _kubernetesClient;

        private DateTime _tokenExpires = DateTime.UtcNow;
        private TokenData _tokenData = null;

        public ConfigurationsMonitor(IConfiguration configuration, ILogger<ConfigurationsMonitor> logger, OIDCSettings oidcSettings, IKubernetesClient kubernetesClient)
        {
            _logger = logger;
            _oidcSettings = oidcSettings;
            _configuration = configuration;
            _kubernetesClient = kubernetesClient;
        }

        private async Task<string> FetchToken()
        {
            if (_tokenData == null || DateTime.UtcNow > _tokenExpires)
            {
                _tokenData = null;
                //Comax.Central.CentralApi centralApi = new Comax.Central.CentralApi();
                TokenClient tokenClient = new TokenClient(_oidcSettings);
                var (success, res) = await tokenClient.AuthenticateClient(_oidcSettings.Scopes);
                if (success)
                {
                    _tokenData = res;
                    _tokenExpires = DateTime.UtcNow.AddSeconds(res.expires_in - 300);
                }
            }
            return _tokenData?.access_token;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var t = await FetchToken();
                    if (!string.IsNullOrWhiteSpace(t))
                    {
                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", t);
                        Comax.Central.CentralApi centralApi = new Comax.Central.CentralApi(_configuration["CentralApi"], client);

                        var specs = await centralApi.GetEcosystemSpecAsync("Comax - Commons");
                        var images = specs.AppSpecs.ToDictionary(x => x.AppType, x => x);
                        var apps = await centralApi.HostedAppsAsync("Commons");

                        foreach (var app in apps)
                        {
                            var ns = await _kubernetesClient.Get<V1Namespace>(app.Namespace);
                            if (ns == null)
                            {
                                await _kubernetesClient.Create(new V1Namespace(metadata: new V1ObjectMeta(name: app.Namespace)));
                            }

                            var conf = await centralApi.AppConfigAsync(app.AppId);
                            var enc = Encoding.UTF8;

                            foreach (var c in conf)
                            {
                                if (c.FromSecret)
                                {
                                    var secret = await centralApi.GetSecretAsync(c.AppId, c.Key);
                                    c.Value = secret.Value;
                                }
                            }

                            var dic = conf.ToDictionary(x => x.Key, x => x.Value);

                            V1Secret v1Secret = new V1Secret(metadata: new V1ObjectMeta(name: "commons-secret", namespaceProperty: app.Namespace),
                                data: new Dictionary<string, byte[]>
                                {
                                    { "OidcSecret", enc.GetBytes(dic["OidcSecret"])},
                                    { "DbCredUsername", enc.GetBytes(dic["DbCredUsername"])},
                                    { "DbCredPassword", enc.GetBytes(dic["DbCredPassword"])},
                                    { "dbrootpw", enc.GetBytes(dic["dbrootpw"])}
                                });

                            var s = await _kubernetesClient.Get<V1Secret>("commons-secret", app.Namespace);
                            if (s == null)
                                await _kubernetesClient.Create(v1Secret);
                            else    
                                await _kubernetesClient.Update(v1Secret);
                            
                            ComaxAgent comaxAgent = new ComaxAgent
                            {
                                ApiVersion = "communaxiom.org/v1alpha1",
                                Kind = "ComaxAgent",
                                Metadata = new V1ObjectMeta(name: "commons-instance", namespaceProperty: app.Namespace),
                                Spec = new ComaxAgentSpec
                                {
                                    RefereeImage = images["AgentReferee"].DockerImage(),
                                    AgentSiloImage = images["Commons"].DockerImage(),
                                    StoreApiImage = images["GrainStorageService"].DockerImage(),
                                    CommonsClientImage = images["CommonsClient"].DockerImage(),
                                    MariadbImage = images["CommonsMariaDb"].DockerImage(),

                                    DbCredPasswordKey = "DbCredPassword",
                                    DbCredRootPasswordKey = "dbrootpw",
                                    DbCredSecretName = "commons-secret",
                                    DbCredUsernameKey = "DbCredUsername",

                                    GatewayPort = dic["GatewayPort"],
                                    SiloPort = dic["SiloPort"],

                                    ClusterId = dic["ClusterId"],
                                    ServiceId = dic["ServiceId"],

                                    IngressCertManager = _configuration["CertManager"],
                                    IngressCertSecret = $"{dic["APP_HASH"]}-ingr-cert",
                                    IngressHost = _configuration["IngressTemplate"].Replace("[HASH]", dic["APP_HASH"]),

                                    MembershipAddress = dic["MembershipHost"],
                                    OidcAuthority = dic["oidcAuthority"],
                                    OidcClientId = dic["OidcClientId"],
                                    OidcSecretKey = "OidcSecret",
                                    OidcSecretName = "commons-secret"
                                },
                                Status = new ComaxAgentState()
                            };

                            await _kubernetesClient.SaveObject(comaxAgent);
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(10));
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error running configuration monitor", ex);
                }
            }
        }
    }
}
