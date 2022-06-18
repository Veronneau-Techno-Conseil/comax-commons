using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Auth;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Net.Http.Headers;
using IdentityModel.OidcClient.Browser;
using System.Threading;
using Orleans.Streams;
using RandomDataGenerator.FieldOptions;
using JWT.Algorithms;
using JWT;
using JWT.Serializers;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.Configuration;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    [StatelessWorker(1)]
    [Reentrant]
    public class AuthenticationWorker : Grain, IAuthentication, IBrowser
    {
        private readonly IConfiguration _configuration;

        private Guid _operationId;
        private IAsyncStream<AuthorizationInstructions> _asyncStream;
        private BrowserResult _browserResult;
        private SessionInfo _sessionInfo;
        private ILogger<AuthenticationWorker> _logger;
        private string clientid;
        private string secret;
        private string redirect;
        private bool _shouldSaveClientCredentials;

        public AuthenticationWorker(IConfiguration configuration, ILogger<AuthenticationWorker> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task Proceed()
        {
            _ = RunAuthentication(clientid, secret, redirect);
            return Task.CompletedTask;
        }

        public async Task Complete()
        {
            _logger.LogInformation("cleaning up");
            await _asyncStream.OnCompletedAsync();
            _operationId = Guid.Empty;
            _browserResult = null;
            _asyncStream = null;
            _sessionInfo = null;

            if (_shouldSaveClientCredentials)
            {
                var account = this.GrainFactory.GetGrain<IAccount>(Guid.Empty);
                await account.Initialize(new AccountDetails
                {
                    ClientID = clientid,
                    ClientSecret = secret
                });
            }

            clientid = null;
            secret = null;
            redirect = null;
            _shouldSaveClientCredentials = false;
        }

        public Task<OperationResult<AuthorizationInstructions>> LaunchServiceAuthentication(string clientId, string clientSecret, string redirectUri)
        {
            _operationId = Guid.NewGuid();
            _asyncStream = this.GetStreamProvider(Constants.DefaultStream).GetStream<AuthorizationInstructions>(_operationId, Constants.DefaultNamespace);
            clientid = clientId;
            secret = clientSecret;
            redirect = redirectUri;
            _shouldSaveClientCredentials = true;
            return Task.FromResult(new OperationResult<AuthorizationInstructions>()
            {
                IsError = false,
                Result = new AuthorizationInstructions()
                {
                    Step = Instruction.LaunchAuthStream,
                    Payload = _operationId.ToString()
                }
            });
        }

        public Task<SessionInfo> GetSessionInfo()
        {
            return Task.FromResult(_sessionInfo);
        }

        private async Task RunAuthentication(string clientId, string clientSecret, string redirectUri)
        {
            try
            {
                _logger.LogInformation("Launched running authentication process");
                var settings = new OIDCSettings();
                _configuration.Bind(Sections.OIDCSection, settings);

                var options = new OidcClientOptions
                {
                    Authority = settings.Authority,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    LoadProfile = false,
                    RedirectUri = redirectUri,
                    Scope = settings.Scopes,
                    Browser = this
                };

                var client = new OidcClient(options);
                _logger.LogInformation("Calling and waiting for login");
                var result = await client.LoginAsync(new LoginRequest());

                if (result.IsError)
                {
                    await _asyncStream.OnNextAsync(new AuthorizationInstructions
                    {
                        Step = Instruction.SetResult,
                        Payload = Newtonsoft.Json.JsonConvert.SerializeObject(new OperationResult
                        {
                            IsError = true,
                            Error = result.Error
                        })
                    });
                    await _asyncStream.OnCompletedAsync();
                    _operationId = Guid.Empty;
                    _browserResult = null;
                    _asyncStream = null;
                    return;
                }

                _logger.LogInformation("Setting user data");
                //Send stream user available
                _sessionInfo = new SessionInfo
                {
                    UserData = Convert(result.User),
                    AccessToken = result.AccessToken,
                    IdentityToken = result.IdentityToken,
                    RefreshToken = result.RefreshToken,
                    AuthenticationExpiration = result.AccessTokenExpiration,
                    AuthenticationTime = result.AuthenticationTime
                };

                await _asyncStream.OnNextAsync(new AuthorizationInstructions
                {
                    Step = Instruction.FetchUser,
                    Payload = ""
                });


                _logger.LogInformation("Setting positive result");
                await _asyncStream.OnNextAsync(new AuthorizationInstructions
                {
                    Step = Instruction.SetResult,
                    Payload = Newtonsoft.Json.JsonConvert.SerializeObject(new OperationResult
                    {
                        IsError = false,
                        Detail = "Authentication successful"
                    })
                }); ;
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication process failed");
            }
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Invoking browser");
            _browserResult = null;
            await _asyncStream.OnNextAsync(new AuthorizationInstructions
            {
                Payload = Newtonsoft.Json.JsonConvert.SerializeObject(options),
                Step = Instruction.OpenUrl
            });

            while (!cancellationToken.IsCancellationRequested)
            {
                if(_browserResult == null)
                {
                    await Task.Delay(250);
                }
                else
                {
                    return _browserResult;
                }
            }
            throw new InvalidOperationException("Operation was cancelled before completing");
        }

        public async Task<OperationResult<AuthorizationInstructions>> LaunchAuthentication(string redirectUri)
        {
            _operationId = Guid.NewGuid();
            _asyncStream = this.GetStreamProvider(Constants.DefaultStream).GetStream<AuthorizationInstructions>(_operationId, Constants.DefaultNamespace);
            var account = this.GrainFactory.GetGrain<IAccount>(Guid.Empty);
            var details = await account.GetDetails();
            clientid = details.ClientID;
            secret = details.ClientSecret;
            redirect = redirectUri;
            _shouldSaveClientCredentials = false;
            return new OperationResult<AuthorizationInstructions>()
            {
                IsError = false,
                Result = new AuthorizationInstructions()
                {
                    Step = Instruction.LaunchAuthStream,
                    Payload = _operationId.ToString()
                }
            };
        }

        public async Task<OperationResult<SessionInfo>> RetrieveToken(string clientId, string clientSecret, string devideCode, int interval)
        {
            using var client = new HttpClient();
            var result = new OperationResult<SessionInfo>();

            IdentityModel.Client.TokenResponse tokenResponse;
            do
            {
                var discovery = await client.GetDiscoveryDocumentAsync(_configuration["oidcUrl"]);
                if (discovery.IsError)
                {
                    result.IsError = true;
                    result.Error = discovery.Error;
                    return result;
                }
                tokenResponse = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = discovery.TokenEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    DeviceCode = devideCode
                });

                if (tokenResponse is { IsError: true, Error: Errors.AuthorizationPending })
                {
                    Console.WriteLine(" - authorization pending...");
                    // Note: `deviceResponse.Interval` is the minimum number of seconds
                    // the client should wait between polling requests.
                    // In this sample the client will retry every 60 seconds at most.

                    await Task.Delay(Math.Min(Math.Max(interval, 1), 60) * 1000);
                }
                else if (tokenResponse.IsError)
                {
                    result.IsError = true;
                    result.Error = tokenResponse.Error;
                    return result;
                }
                else
                {
                    var account = this.GrainFactory.GetGrain<IAccount>(Guid.Empty);
                    await account.Initialize(new AccountDetails
                    {
                        ClientID = clientId,
                        ClientSecret = clientSecret,
                    });

                    result.Result = new SessionInfo
                    {
                        AccessToken = tokenResponse.AccessToken,
                        IdentityToken = tokenResponse.IdentityToken,
                        RefreshToken = tokenResponse.RefreshToken
                    };

                    return result;
                }
            }
            while (true);
        }

        public Task SetResult(Contracts.Remote.BrowserResult browserResult)
        {
            _browserResult = new BrowserResult
            {
                Error = browserResult.Error,
                ErrorDescription = browserResult.ErrorDescription,
                Response = browserResult.Response,
                ResultType = (BrowserResultType)browserResult.ResultType
            };
            return Task.CompletedTask;
        }

        public byte[] Convert(ClaimsPrincipal claimsPrincipal)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            claimsPrincipal.WriteTo(bw);
            ms.Position = 0;
            return ms.ToArray();            
        }
    }
}
