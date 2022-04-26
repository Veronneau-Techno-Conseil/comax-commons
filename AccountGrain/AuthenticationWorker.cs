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

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    [StatelessWorker(1)]
    [Reentrant]
    public class AuthenticationWorker : Grain, IAuthentication
    {
        private readonly IConfiguration _configuration;

        public AuthenticationWorker(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public async Task<OperationResult<AuthorizationInstructions>> LaunchAuthentication(string clientId, string clientSecret)
        {
            throw new NotImplementedException();
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
    }
}
