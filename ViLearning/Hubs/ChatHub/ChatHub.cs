using Microsoft.AspNetCore.SignalR;
using ViLearning.Models;

namespace ViLearning.Hubs.ChatHub
{
    public class ChatHub : Hub
    {
        /*public async Task SendMessage(ApplicationUser receiver, ApplicationUser sender, string message)
        {
            Clients.User(receiver.Id).SendAsync("ReceiveMessage", sender, message, DateTime.Now);
        }*/

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now);
        }
    }
}
