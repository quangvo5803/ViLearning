using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Conversation> GetOrCreateConversation(string user1Id, string user2Id)
        {
            var conversation = _unitOfWork.Conversation
                .Get(c => (c.User1Id == user1Id && c.User2Id == user2Id) ||
                          (c.User1Id == user2Id && c.User2Id == user1Id));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1Id = user1Id,
                    User2Id = user2Id,
                };

                _unitOfWork.Conversation.Add(conversation);
                _unitOfWork.Save();
            }

            return conversation;
        }

        public async Task SaveMessage(int conversationId, string senderId, string messageText)
        {
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                MessageText = messageText,
                Timestamp = DateTime.Now
            };

            _unitOfWork.Message.Add(message);

            var conversation = _unitOfWork.Conversation.Get(c => c.ConversationId == conversationId);
            conversation.LastMessage = message;
            conversation.LastMessageId = message.MessageId;

            _unitOfWork.Save();
        }
    }
}
