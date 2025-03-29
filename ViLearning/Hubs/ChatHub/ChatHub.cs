using Microsoft.AspNetCore.SignalR;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Hubs.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }
        public async Task SendMessageTo(string receiverId, string senderId, string message)
        {
            var sendAt = DateTime.Now.ToString("HH:mm | MMM d");
            // Get or create the conversation
            var conversation = await _chatService.GetOrCreateConversation(senderId, receiverId);

            // Save the message
            await _chatService.SaveMessage(conversation.ConversationId, senderId, message);
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message, sendAt);
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, message, sendAt);
        }

        public async Task SendMessage(string userId, string message)
        {
            var sendAt = DateTime.Now.ToString("HH:mm | MMM d");
            await Clients.All.SendAsync("ReceiveMessage",userId, message, sendAt);
        }
    }
}
