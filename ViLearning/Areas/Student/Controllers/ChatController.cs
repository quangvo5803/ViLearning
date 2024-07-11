using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;
using ViLearning.Models;

namespace ViLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = SD.Role_User_Student + "," + SD.Role_User_Teacher)]
    public class ChatController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BlobStorageService _blobStorageService;
        public ChatController(IUnitOfWork unitOfWork, BlobStorageService blobStorageService)
        {
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["userId"] = userId;
            return View();
        }

        public async Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id)
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

        public async Task SaveMessageAsync(int conversationId, string senderId, string messageText)
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

            _unitOfWork.Save();
        }
    }
}
