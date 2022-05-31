using Blazored.Toast.Services;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.Services;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using CommunAxiom.Commons.Client.Contracts;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels
{
    public class SessionViewModel : ISessionViewModel
    {
        public string? ClientId { get; set; }
        public string? Secret { get; set; }
        public LoginType LoginType { get; set; }

        private readonly HttpClient _httpClient;
        private readonly IToastService _toastService;
        private readonly IStringLocalizer<SessionViewModel> _stringLocalizer;
        private readonly IStdMessagesService _stdMessagesService;
        private readonly IAccessTokenService _accessTokenService;
        public SessionViewModel(HttpClient httpClient, IToastService toastService, IStringLocalizer<SessionViewModel> stringLocalizer, IStdMessagesService stdMessagesService, IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _toastService = toastService;
            _stringLocalizer = stringLocalizer;
            _stdMessagesService = stdMessagesService;
            _accessTokenService = accessTokenService;
        }

        public async Task<OperationResult<string>> GetState()
        {
            try
            {
                var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
                if (!string.IsNullOrWhiteSpace(jwtToken)) 
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                var msg = await _httpClient.GetAsync("authentication");
                
                var resString = await msg.Content.ReadAsStringAsync();

                if (msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new OperationResult<string> { IsError = true, Error = OperationResult.ERR_UNEXP_NULL, Detail = resString };
                }

                var res = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationResult<string>>(resString);

                return res ?? new OperationResult<string> { IsError = true, Error = OperationResult.ERR_UNEXP_NULL };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<User?> Login()
        {
            var res = await GetState();
            if (res.IsError)
                _stdMessagesService.ToastError(res.Error, _stringLocalizer["Authentication"], _stringLocalizer["StateAction"]);
            HttpResponseMessage? httpResponseMessage;

            if (res.Result == AuthSteps.AuthApi || res.Result == AuthSteps.Reset)
            {
                httpResponseMessage = await _httpClient.PostAsJsonAsync<AuthStart>("authentication/cluster", new AuthStart { ClientId = ClientId, ClientSecret = Secret });
            }
            else
            {
                httpResponseMessage = await _httpClient.PostAsync("authentication", null);
            }

            if (httpResponseMessage != null)
            {
                var authRes = await httpResponseMessage.Content.ReadFromJsonAsync<OperationResult<AuthResult>>();
                if (authRes != null && authRes.Result != null && !string.IsNullOrWhiteSpace(authRes.Result.Token))
                {
                    await _accessTokenService.SetAccessTokenAsync("jwt_token", authRes.Result.Token);
                }
                else
                {
                    throw new Exception("Result with token expected from authentication");
                }
            }

            return await GetUserByJWTAsync();
        }


        public async Task<User?> GetUserByJWTAsync()
        {
            try
            {
                var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");

                if(string.IsNullOrWhiteSpace(jwtToken)) return null;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                //making the http request
                var returnedUser = await _httpClient.GetFromJsonAsync<User>("user");


                if (returnedUser != null) return returnedUser;
                else return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType());
                return null;
            }
        }

    }
}
