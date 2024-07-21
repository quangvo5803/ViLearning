using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IChatService
    {
        Task<Conversation> GetOrCreateConversation(string user1Id, string user2Id);
        Task SaveMessage(int conversationId, string senderId, string messageText);
    }
}
