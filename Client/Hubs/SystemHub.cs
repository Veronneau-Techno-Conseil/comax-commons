using CommunAxiom.Commons.ClientUI.Shared.Resources;
using Microsoft.AspNetCore.SignalR;

namespace CommunAxiom.Commons.ClientUI.Server.Hubs
{
    public class SystemHub : Hub
    {
        public async Task SendNotification(Notifications notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }
    }
}
