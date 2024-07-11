using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using ViLearning.Models;

namespace ViLearning.Hubs.ChatHub
{
    public class ChatHub : Hub
    {
        /*public async Task SendMessage(ApplicationUser receiver, ApplicationUser sender, string message)
        {
            Clients.User(receiver.Id).SendAsync("ReceiveMessage", sender, message, DateTime.Now);
        }*/

        public async Task SendMessage(string userId, string message)
        {
            var sendAt = DateTime.Now.ToString("HH:mm | MMM d");
            await Clients.All.SendAsync("ReceiveMessage",userId, message, sendAt);
        }
    }
}
