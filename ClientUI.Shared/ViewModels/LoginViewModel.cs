using Blazored.Toast.Services;
using ClientUI.Shared.Models;
using ClientUI.Shared.Services;
using ClientUI.Shared.ViewModels.Interfaces;
using CommunAxiom.Commons.Client.Contracts;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Shared.ViewModels
{
    public class LoginViewModel : ILoginViewModel
    {
        public string? ClientId { get; set; }
        public string? Secret { get; set; }
        public LoginType LoginType { get; set; }


        private readonly HttpClient _httpClient;
        private readonly IToastService _toastService;
        private readonly IStringLocalizer<LoginViewModel> _stringLocalizer;
        private readonly IStdMessagesService _stdMessagesService;
        private readonly IAccessTokenService _accessTokenService;
        public LoginViewModel(HttpClient httpClient, IToastService toastService, IStringLocalizer<LoginViewModel> stringLocalizer, IStdMessagesService stdMessagesService, IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _toastService = toastService;
            _stringLocalizer = stringLocalizer;
            _stdMessagesService = stdMessagesService;
            _accessTokenService = accessTokenService;
        }

        public async Task<OperationResult<string>> GetState()
        {
            return (await _httpClient.GetFromJsonAsync<OperationResult<string>>("authentication")) ?? new OperationResult<string> { IsError = true, Error = OperationResult.ERR_UNEXP_NULL };
        }

        public async Task Login(LoginType loginType)
        {
            var res = await GetState();
            if (res.IsError)
                _stdMessagesService.ToastError(res.Error, _stringLocalizer["Authentication"], _stringLocalizer["StateAction"]);

            if(res.Result == AuthSteps.AuthApi || res.Result == AuthSteps.Reset)
            {
                await _httpClient.PostAsJsonAsync<AuthStart>("authentication/cluster", new AuthStart { ClientId = ClientId, ClientSecret = Secret });
            }
        }

        private async Task HandleAuthResult(HttpResponseMessage httpResponseMessage)
        {
            var res = await httpResponseMessage.Content.ReadFromJsonAsync<OperationResult<string>>() ?? new OperationResult<string> { IsError = true, Error = OperationResult.ERR_UNEXP_NULL };
            if (res.IsError)
                _stdMessagesService.ToastError(res.Error, _stringLocalizer["Authentication"], _stringLocalizer["StateAction"]);
            else
            {
                string token = res.Result;
                await _accessTokenService.SetAccessTokenAsync("JWT", token);
            }
        }
    }
}
