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
using CommunAxiom.Commons.ClientUI.Server.Helper;
using Microsoft.Extensions.Configuration;
using CommunAxiom.Commons.Client.Contracts.Configuration;
using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Account;
using Orleans.Streams;
using IdentityModel.OidcClient.Browser;
using System.Threading;
using CommunAxiom.Commons.Client.Contracts.Remote;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;

namespace CommunAxiom.Commons.ClientUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private static string _operationId = "";
        
        private readonly ITempData _tempData;
        private readonly IConfiguration _configuration; 
        private readonly IServiceProvider _serviceProvider;
        public AuthenticationController(ITempData tempData, IConfiguration configuration, IServiceProvider serviceProvider)
        {
        
            _tempData = tempData;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("values")]
        [Authorize]
        public ActionResult<IEnumerable<string>> GetValues()
        {
            var currentUser = HttpContext.User;
            int spendingTimeWithCompany = 0;

            if (currentUser.HasClaim(c => c.Type == "DateOfJoing"))
            {
                DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DateOfJoing").Value);
                spendingTimeWithCompany = DateTime.Today.Year - date.Year;
            }

            if (spendingTimeWithCompany > 5)
            {
                return new string[] { "High Time1", "High Time2", "High Time3", "High Time4", "High Time5" };
            }
            else
            {
                return new string[] { "value1", "value2", "value3", "value4", "value5" };
            }
        }

        [HttpGet()]
        public async Task<IActionResult> Get([FromServices] ICommonsClientFactory clusterClient)
        {
            var state = await clusterClient.WithClusterClient(async cc =>
            {
                var act = cc.GetAccount();
                return await act.CheckState(true);
            });

            if (state == AccountState.Initial || state == AccountState.AuthenticationError)
            {
                return Ok(new OperationResult<string>()
                {
                    Detail = "Full service auth required.",
                    IsError = false,
                    Result = AuthSteps.AuthApi
                });
            }

            if (this.User.Identity.IsAuthenticated)
            {
                return Ok(new OperationResult<string>()
                {
                    Detail = "All good.",
                    IsError = false,
                    Result = AuthSteps.OK
                });
            }

            if(state == AccountState.CredentialsSet)
            {
                return Ok(new OperationResult<string>()
                {
                    Detail = "Authenticate the user.",
                    IsError = false,
                    Result = AuthSteps.LOGIN
                });
            }

            throw new InvalidOperationException("Unexpected authentication state obtained.");
        }

        [HttpGet("Login")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login()
        {
            if (Request.Method == "GET")
            {
                return await SetResultAsync(Request.QueryString.Value, HttpContext);
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
                        return await SetResultAsync(body, HttpContext);
                    }
                }
            }
            else
            {
                return StatusCode(405);
            }
        }

        private async Task<IActionResult> SetResultAsync(string value, HttpContext ctx)
        {
            try
            {
                ctx.Response.ContentType = "text/html";
                await ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
                await ctx.Response.Body.FlushAsync();
                _tempData.SetOperationResult(_operationId, value);
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

        [HttpPost("cluster")]
        public async Task<IActionResult> AuthenticateCluster(AuthStart auth, [FromServices]ICommonsClientFactory clusterClient, CancellationToken cancellationToken)
        {
            //Ensure state is valid
            return await clusterClient.WithClusterClient(async cc =>
            {
                try
                {
                    var act = cc.GetAccount();
                    var state = await act.CheckState(false, auth.ClientId);

                    if (state == Client.Contracts.Account.AccountState.ClientMismatch)
                    {
                        //Should reinisitalize the whole cluster is clientId mismatch, clientid is supposed to be permanent and only secret changes
                        return Unauthorized(new OperationResult<string>()
                        {
                            Detail = "Cannot authenticate against a different client id than that which is set in commons.",
                            Error = AuthSteps.ERR_ClientMismatch,
                            IsError = true,
                            Result = AuthSteps.LOGIN
                        });
                    }

                    //Launch auth with redirect url in ref
                    var authSvc = cc.GetAuthentication();
                    string redirectUri = string.Format($"https://localhost:{Request.Host.Port}/api/authentication/login");
                    var instructions = await authSvc.LaunchServiceAuthentication(auth.ClientId, auth.ClientSecret, redirectUri);

                    return await CompleteAuthentication(cc, authSvc, instructions, cancellationToken);
                }
                catch (Exception ex)
                {
                    return Ok(new OperationResult<string>()
                    {
                        Detail = ex.Message,
                        Error = AuthSteps.ERR_Unexpected,
                        IsError = true,
                        Result = AuthSteps.ERR_Unexpected
                    });
                }
            });
        }


        [HttpPost()]
        public async Task<IActionResult> Authenticate([FromServices] ICommonsClientFactory clusterClient, CancellationToken cancellationToken)
        {
            var (client, result) = await clusterClient.WithUnmanagedClient(async cc =>
            {
                //Ensure state is valid
                var act = cc.GetAccount();
                var state = await act.CheckState(false);

                if (state == Client.Contracts.Account.AccountState.Initial)
                {
                    //Should reinisitalize the whole cluster is clientId mismatch, clientid is supposed to be permanent and only secret changes
                    return Unauthorized(new OperationResult<string>()
                    {
                        Detail = "You must authenticate the cluster first.",
                        Error = AuthSteps.ERR_Authentication,
                        IsError = true,
                        Result = AuthSteps.LOGIN
                    });
                }

                //Launch auth with redirect url in ref
                var authSvc = cc.GetAuthentication();
                string redirectUri = string.Format($"https://localhost:{Request.Host.Port}/api/authentication/login");
                var instructions = await authSvc.LaunchAuthentication(redirectUri);

                return await CompleteAuthentication(cc, authSvc, instructions, cancellationToken);
            });

            return result;
        }

        private async Task<IActionResult> CompleteAuthentication(ICommonsClusterClient clusterClient, IAuthentication authSvc, OperationResult<AuthorizationInstructions> instructions, CancellationToken cancellationToken)
        {
            if (instructions.IsError)
            {
                return this.StatusCode(500, new OperationResult<string>()
                {
                    Detail = $"Cannot authenticate because of cluster error: {instructions.Detail}",
                    Error = AuthSteps.ERR_Unexpected,
                    IsError = true,
                    Result = AuthSteps.LOGIN
                });
            }

            //First step should always to launch synchronization stream
            if (instructions.Result.Step != Instruction.LaunchAuthStream)
            {
                throw new InvalidOperationException("Expected lauch stream instructions");
            }

            //Payload is the stream's id and also the ref id for the browser
            _operationId = instructions.Result.Payload;
            var (handle, enumerable) = await clusterClient.SubscribeAuth(Guid.Parse(_operationId));
            await authSvc.Proceed();
            SessionInfo sessionInfo = null;
            OperationResult operationResult = null;

            await foreach (var item in enumerable.WithCancellation(cancellationToken))
            {
                switch (item.Step)
                {
                    case Instruction.LaunchAuthStream:
                        continue;
                    case Instruction.OpenUrl:
                        // The client's browser is delegated to the cluster in order to complete the authorization process, this is reflected by a virtual browser in the cluster
                        SystemBrowser systemBrowser = new SystemBrowser(_serviceProvider, _operationId);
                        var options = Newtonsoft.Json.JsonConvert.DeserializeObject<BrowserOpts>(item.Payload);

                        var browserRes = await systemBrowser.InvokeAsync(new BrowserOptions(options.StartUrl, options.EndUrl) { DisplayMode = (DisplayMode)options.DisplayMode, Timeout = options.Timeout });
                        await authSvc.SetResult(new Client.Contracts.Remote.BrowserResult
                        {
                            Error = browserRes.Error,
                            ErrorDescription = browserRes.ErrorDescription,
                            Response = browserRes.Response,
                            ResultType = (int)browserRes.ResultType
                        });

                        break;
                    case Instruction.SetResult:
                        operationResult = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationResult>(item.Payload);
                        if (!operationResult.IsError)
                        {
                            await authSvc.Complete();
                        }
                        break;
                    case Instruction.FetchUser:
                        sessionInfo = await authSvc.GetSessionInfo();
                        break;
                }
            }

            await clusterClient.Close();
            clusterClient.Dispose();

            if (operationResult.IsError)
            {
                return this.Ok(new OperationResult<string>()
                {
                    Result = AuthSteps.LOGIN,
                    Error = AuthSteps.ERR_Authentication,
                    IsError = true,
                    Detail = $"Error during authentication: {operationResult.Error}"
                });
            }

            var data = new TokenData
            {
                AccessToken = sessionInfo.AccessToken,
                AuthenticationTime = (sessionInfo.AuthenticationTime ?? DateTimeOffset.Now).UtcDateTime,
                AccessTokenExpiration = sessionInfo.AuthenticationExpiration.UtcDateTime,
                IdentityToken = sessionInfo.IdentityToken,
                RefreshToken = sessionInfo.RefreshToken
            };

            _tempData.SetTokenData(data);
            MemoryStream ms = new MemoryStream(sessionInfo.UserData);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new BinaryReader(ms));
            string token = GenerateJSONWebToken(claimsPrincipal, sessionInfo.AuthenticationExpiration.DateTime);

            return this.Ok(new OperationResult<AuthResult>
            {
                IsError = false,
                Result = new AuthResult { Token = token}
            });
        }

        private string GenerateJSONWebToken(ClaimsPrincipal userInfo, DateTime expiration)
        {
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                       

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: userInfo.Claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
