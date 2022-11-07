using CommunAxiom.Commons.ClientUI.Shared;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

namespace ClientUI.Components.Authentication
{
    public partial class Login
    {
        [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
        [Parameter] public bool RegisterRedirect { get; set; }

        private bool IsLoading { get; set; }

        private bool AuthenticateCluster { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationState;

            if (authState.User.Identity is { IsAuthenticated: true })
            {
                _navigationManager.NavigateTo("/");
            }

            var state = await _loginViewModel.GetState();
            switch (state.Result)
            {
                case AuthSteps.Reset:
                case AuthSteps.AuthApi:
                    AuthenticateCluster = true;
                    break;
                default:
                    AuthenticateCluster = false;
                    break;
            }
        }

        private async Task OnKeyPress(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                await AuthenticateJWT();
        }

        private async Task AuthenticateJWT()
        {
            IsLoading = true;

            var authenticationResponse = await _loginViewModel.Login();
            if (authenticationResponse != null)
            {
                await ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated();

                if (_applicationSettings.Value.UIFramework == "Server")
                    _navigationManager.NavigateTo("/");
                else
                    _navigationManager.NavigateTo("/", true);
            }
            else
            {
                _toastService.ShowError("Invalid username or password");
            }

            IsLoading = false;
        }
    }
}