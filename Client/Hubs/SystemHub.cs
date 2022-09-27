using Microsoft.AspNetCore.SignalR;

namespace CommunAxiom.Commons.ClientUI.Server.Hubs
{
    public class SystemHub : Hub
    {
        public async Task SendNotification(string title, string body, int criticality)
        {
            await Clients.All.SendAsync("ReceiveNotification", title, body, criticality);
        }
    }
}
