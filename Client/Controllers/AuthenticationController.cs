using CommunAxiom.Commons.Client.Contracts.Auth;
using IdentityModel.OidcClient;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using Orleans;
using System;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using CommunAxiom.Commons.ClientUI.Helper;
using Microsoft.Extensions.Configuration;
using CommunAxiom.Commons.Client.Contracts.Configuration;
using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.ClientUI.ApiContracts;

namespace CommunAxiom.Commons.ClientUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly Client.Contracts.Grains.ICommonsClusterClient _clusterClient;
        private readonly ITempData _tempData;
        private readonly IConfiguration _configuration; 
        private readonly IServiceProvider _serviceProvider;
        public AuthenticationController(Client.Contracts.Grains.ICommonsClusterClient clusterClient, ITempData tempData, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _clusterClient = clusterClient;
            _tempData = tempData;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("Login/{callbackid}")]
        [HttpPost("Login/{callbackid}")]
        public async Task<IActionResult> Login(string callbackid)
        {
            if (Request.Method == "GET")
            {
                return await SetResultAsync(callbackid, Request.QueryString.Value, HttpContext);
            }
            else if (Request.Method == "POST")
            {
                if (!Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                {
                    return StatusCode(415);
                }
                else
                {
                    using (var sr = new StreamReader(Request.Body, Encoding.UTF8))
                    {
                        var body = await sr.ReadToEndAsync();
                        return await SetResultAsync(callbackid, body, HttpContext);
                    }
                }
            }
            else
            {
                return StatusCode(405);
            }
        }

        private async Task<IActionResult> SetResultAsync(string callbackid, string value, HttpContext ctx)
        {
            try
            {
                
                ctx.Response.ContentType = "text/html";
                await ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
                await ctx.Response.Body.FlushAsync();
                _tempData.SetOperationResult(callbackid,value);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                ctx.Response.ContentType = "text/html";
                await ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
                await ctx.Response.Body.FlushAsync();
                return StatusCode(400);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> Authenticate(AuthStart auth)
        {
            var act = _clusterClient.GetAccount();
            var state = await act.CheckState(auth.ClientId);

            if(state == Client.Contracts.Account.AccountState.ClientMismatch)
            {
                return Ok(new OperationResult<string>()
                {
                    Detail = "Cannot authenticate against a different client id than that which is set in commons.",
                    Error = ApiContracts.AuthSteps.ERR_ClientMismatch,
                    IsError = true,
                    Result = ApiContracts.AuthSteps.LOGIN
                });
            }

            var settings = new OIDCSettings();
            _configuration.Bind(Sections.OIDCSection, settings);
            string callback = Guid.NewGuid().ToString();

            string redirectUri = string.Format($"http://127.0.0.1:{Request.Host.Port}/Authentication/Login/{callback}");
            var options = new OidcClientOptions
            {
                Authority = settings.Authority,
                ClientId = auth.ClientId,
                LoadProfile = false,
                RedirectUri = redirectUri,
                Scope = StandardScopes.OpenId,
                Browser = new SystemBrowser(_serviceProvider, callback)
            };
            
            var client = new OidcClient(options);
            var result = await client.LoginAsync(new LoginRequest());

            if (result.IsError)
            {
                return this.Ok(new OperationResult<string>()
                {
                  Result = ApiContracts.AuthSteps.LOGIN,
                  Error = ApiContracts.AuthSteps.ERR_Authentication,
                  IsError=true,
                  Detail = $"Error during authentication: {result.Error} - {result.ErrorDescription}"
                });
            }

            var data = new TokenData
            {
                AccessToken = result.AccessToken,
                AuthenticationTime = result.AuthenticationTime.Value.UtcDateTime,
                AccessTokenExpiration = result.AccessTokenExpiration.UtcDateTime,
                IdentityToken = result.IdentityToken,
                RefreshToken = result.RefreshToken
            };

            _tempData.SetTokenData(data);
            

            var next = ApiContracts.AuthSteps.OK;
            var resetClientSecret = new[] { AccountState.Initial, AccountState.AuthenticationError };
            if (resetClientSecret.Contains(state))
            {
                next = ApiContracts.AuthSteps.ApiSecret;
            }

            return this.Ok(new OperationResult<string>()
            {
                Result = next,
                Error = ApiContracts.AuthSteps.ERR_Authentication,
                IsError = false,
                Detail = $"Error during authentication: {result.Error} - {result.ErrorDescription}"
            });
        }
    }
}
