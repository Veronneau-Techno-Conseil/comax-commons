using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientUI.Components.Dashboard
{
    public partial class Dashboard : IAsyncDisposable
    {
        private HubConnection? hubConnection;
        private List<DashboardItem> notifications = new List<DashboardItem>();

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/systemhub"))
                .Build();

            hubConnection.On<DashboardItem>("ReceiveNotification", (notification) =>
            {
                notifications.Add(notification);
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

        protected override void OnInitialized()
        {
            notifications.Add(new DashboardItem
            {
                Title = "Réginald Bouvier Leduc le Second",
                CreateDateTime = new DateTime(),
                Body = "Réginald Bouvier Leduc le Second requests to add you to their Intacts.",
                Criticality = ItemCriticality.None,
                ItemGroup = ItemGroup.Task
            });
            
            notifications.Add(new DashboardItem
            {
                Title = "Datasource 4",
                Body = "Upcoming ingestion for datasource.",
                Criticality = ItemCriticality.None,
                CreateDateTime = new DateTime(),
                ItemGroup = ItemGroup.UpcomingActivity
            });
            
            notifications.Add(new DashboardItem
            {
                Title = "Datasource 5",
                Body = "Upcoming ingestion for datascources.",
                Criticality = ItemCriticality.None,
                CreateDateTime = new DateTime(),
                ItemGroup = ItemGroup.UpcomingActivity
            });
            
            notifications.Add(new DashboardItem
            {
                Title = "Datasource 5",
                Body = "Upcoming ingestion for datascources.",
                Criticality = ItemCriticality.None,
                CreateDateTime = new DateTime(),
                ItemGroup = ItemGroup.UpcomingActivity
            });
            
            notifications.Add(new DashboardItem
            {
                Title = "Datasource 6",
                Body = "Ingestion in progress...",
                Criticality = ItemCriticality.None,
                CreateDateTime = new DateTime(),
                ItemGroup = ItemGroup.ActivityStream
            });

            for (var i = 0; i < 6; i++)
            {
                 notifications.Add(new DashboardItem 
                 {
                     Title = $"Datasource {i}", 
                     Body = "Ingestion in progress...",
                     Criticality = ItemCriticality.None,
                     CreateDateTime = new DateTime(),
                     ItemGroup = ItemGroup.ActivityStream 
                 });
            }
            
            notifications.Add(new DashboardItem
            {
                Title = "Datasource 1",
                Body = "Ingestion in progress...",
                Criticality = ItemCriticality.None,
                CreateDateTime = new DateTime(),
                ItemGroup = ItemGroup.ActivityStream
            });
            
        }
    }
}