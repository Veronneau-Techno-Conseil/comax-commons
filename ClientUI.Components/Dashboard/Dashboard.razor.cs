using CommunAxiom.Commons.ClientUI.Shared.Resources;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientUI.Components.Dashboard
{
    public partial class Dashboard : IAsyncDisposable
    {
        private HubConnection? hubConnection;
        private List<string> notifications = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/systemhub"))
                .Build();

            hubConnection.On<Notifications>("ReceiveNotification", (notification) =>
            {
                var encodedMsg = $"{notification.Title}: {notification.Body}";
                notifications.Add(encodedMsg);
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }
}