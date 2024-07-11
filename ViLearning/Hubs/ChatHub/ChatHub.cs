using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using ViLearning.Models;

namespace ViLearning.Hubs.ChatHub
{
    public class ChatHub : Hub
    {
        /*public async Task SendMessage(string receiverId, string senderId, string message)
        {
            var sendAt = DateTime.Now.ToString("HH:mm | MMM d");
            Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message, sendAt);
        }*/

        public async Task SendMessage(string userId, string message)
        {
            var sendAt = DateTime.Now.ToString("HH:mm | MMM d");
            await Clients.All.SendAsync("ReceiveMessage",userId, message, sendAt);
        }
    }
}
