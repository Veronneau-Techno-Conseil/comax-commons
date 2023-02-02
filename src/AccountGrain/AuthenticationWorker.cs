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
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    [StatelessWorker(1)]
    [Reentrant]
    public class AuthenticationWorker : Grain, IAuthentication, IBrowser
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationWorker> _logger;
        private readonly ISettingsProvider _settingsProvider;

        private Guid _operationId;
        private IAsyncStream<AuthorizationInstructions> _asyncStream;
        private BrowserResult _browserResult;
        private SessionInfo _sessionInfo;
        private string redirect;

        public AuthenticationWorker(IConfiguration configuration, ILogger<AuthenticationWorker> logger, ISettingsProvider settingsProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _settingsProvider = settingsProvider;
        }

        [AuthorizePassthrough]
        public Task Proceed()
        {
            _ = RunAuthentication(redirect);
            return Task.CompletedTask;
        }

        [AuthorizePassthrough]
        public async Task Complete()
        {
            _logger.LogInformation("cleaning up");
            await _asyncStream.OnCompletedAsync();
            _operationId = Guid.Empty;
            _browserResult = null;
            _asyncStream = null;
            _sessionInfo = null;
            redirect = null;
        }

        [AuthorizePassthrough]
        public Task<SessionInfo> GetSessionInfo()
        {
            return Task.FromResult(_sessionInfo);
        }

        private async Task RunAuthentication(string redirectUri)
        {
            try
            {
                _logger.LogInformation("Launched running authentication process");
                var settings = await _settingsProvider.GetOIDCSettings();

                var options = new OidcClientOptions
                {
                    Authority = settings.Authority,
                    ClientId = settings.ClientId,
                    ClientSecret = settings.Secret,
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

        [AuthorizePassthrough]
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

        [AuthorizePassthrough]
        public Task<OperationResult<AuthorizationInstructions>> LaunchAuthentication(string redirectUri)
        {
            _operationId = Guid.NewGuid();
            _asyncStream = this.GetStreamProvider(Orleans.OrleansConstants.Streams.DefaultStream).GetStream<AuthorizationInstructions>(_operationId, Orleans.OrleansConstants.StreamNamespaces.DefaultNamespace);
            
            redirect = redirectUri;
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

        [AuthorizePassthrough]
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

        [AuthorizePassthrough]
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
