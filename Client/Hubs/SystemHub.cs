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
    }
}
