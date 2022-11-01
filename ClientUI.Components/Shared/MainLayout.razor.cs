using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace ClientUI.Components.Shared
{
    public partial class MainLayout
    {
        private HubConnection? _hubConnection;

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/systemhub"))
                .Build();
            await _hubConnection.StartAsync();

            _hubConnection.On<DashboardItem>("ReceiveNotification", (notification) =>
            {
                var encodedMsg = $"{notification.Title}: {notification.Body}";
                if (notification.Criticality == ItemCriticality.Info)
                {
                    toastService.ShowInfo(encodedMsg);
                }
                else if (notification.Criticality == ItemCriticality.Warning)
                {
                    toastService.ShowWarning(encodedMsg);
                }
                else if (notification.Criticality == ItemCriticality.Success)
                {
                    toastService.ShowSuccess(encodedMsg);
                }
                else
                {
                    toastService.ShowError(encodedMsg);
                }

                StateHasChanged();
            });
        }

        protected override Task OnParametersSetAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsRuntime.InvokeVoidAsync("Startup");
        
        }

        private async Task LogoutUser()
        {
            await _accessTokenService.RemoveAccessTokenAsync("jwt_token");
            _navigationManager.NavigateTo("/", true);
        }
    }
}