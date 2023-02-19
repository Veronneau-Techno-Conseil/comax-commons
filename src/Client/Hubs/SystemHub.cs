using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace CommunAxiom.Commons.ClientUI.Server.Hubs
{
    public class SystemHub : Hub
    {
        public async Task SendNotification(DashboardItem notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
