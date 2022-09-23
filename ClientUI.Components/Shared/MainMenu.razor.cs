using CommunAxiom.Commons.ClientUI.Shared;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ClientUI.Components.Shared
{
    public partial class MainMenu
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

        ElementReference navMenuIndex;

        public User? User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationState;
            this.User = await ((CustomAuthenticationStateProvider)_stateProvider).GetUserByJWTAsync();
        }

        private async Task Logout()
        {
            await _accessTokenService.RemoveAccessTokenAsync("jwt_token");
            _navigationManager.NavigateTo("/", true);
        }

        private void BurgerClick()
        {

        }
    }
}