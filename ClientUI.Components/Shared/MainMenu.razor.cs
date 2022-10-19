using CommunAxiom.Commons.ClientUI.Shared;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientUI.Components.Shared
{
    public partial class MainMenu
    {
        private List<string> criticalNotifications = new List<string>();
        private HubConnection? hubConnection;
        private bool portfolioSubMenu;
        private string notificationsBox = "none";

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

        ElementReference navMenuIndex;

        public User? User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationState;
            this.User = await ((CustomAuthenticationStateProvider)_stateProvider).GetUserByJWTAsync();
            hubConnection = new HubConnectionBuilder()
              .WithUrl(_navigationManager.ToAbsoluteUri("/systemhub"))
              .Build();
            await hubConnection.StartAsync();

            hubConnection.On<Notifications>("ReceiveNotification", (notification) =>
            {
                var encodedMsg = $"{notification.Title}: {notification.Body}";
                if (notification.Criticality == Criticality.Critical)
                {
                    criticalNotifications.Add(encodedMsg);
                }
                StateHasChanged();
            });
        }

        private async Task Logout()
        {
            await _accessTokenService.RemoveAccessTokenAsync("jwt_token");
            _navigationManager.NavigateTo("/", true);
        }

        private void HideShowNotifications()
        {
            if (notificationsBox == "none")
                notificationsBox = "block";
            else
                notificationsBox = "none";
        }

    }
}