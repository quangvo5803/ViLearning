using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

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
    }
}
