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
        public ChatController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["userId"] = userId;

            // Lấy tất cả các cuộc trò chuyện của người dùng và sắp xếp theo thời gian tin nhắn gần nhất
            var conversations = _unitOfWork.Conversation
                .GetAll()
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Select(c => new
                {
                    Conversation = c,
                    LastMessageTimestamp = c.LastMessage != null ? c.LastMessage.Timestamp : (DateTime?)null
                })
                .OrderByDescending(x => x.LastMessageTimestamp)
                .Select(x => x.Conversation) // Trả về đối tượng Conversation
                .ToList();
            foreach (var conversation in conversations)
            {
                conversation.User1 = _unitOfWork.ApplicationUser.Get(u => u.Id == conversation.User1Id);
                conversation.User2 = _unitOfWork.ApplicationUser.Get(u => u.Id == conversation.User2Id);
                conversation.LastMessage = _unitOfWork.Message.Get(m => m.MessageId == conversation.LastMessageId);
            }
            return View(conversations);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMessages(int conversationId)
        {
            var messages = _unitOfWork.Message
                .GetRange(m => m.ConversationId == conversationId)
                .OrderBy(m => m.Timestamp)
                .ToList();

            var messageDtos = messages.Select(m => new
            {
                MessageId = m.MessageId,
                MessageText = m.MessageText,
                Timestamp = m.Timestamp
            });

            return Json(messageDtos);
        }
    }
}
