using Microsoft.AspNetCore.SignalR;

namespace CommunAxiom.Commons.ClientUI.Server.Hubs
{
    public class SystemHub : Hub
    {
        public async Task SendNotification(string title, string body)
        {
            await Clients.All.SendAsync("ReceiveNotification", title, body);
        }
    }
}
