using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = SD.Role_User_Teacher)]
    public class FeedbackController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public FeedbackController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var feedback = _unitOfWork.Feedback.GetRange(f => f.Course.ApplicationUser.Id == userId,includeProperties:"Course").ToList();
            return View(feedback);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = _userManager.GetUserId(User);
            var feedback = _unitOfWork.Feedback
        .GetRange(f => f.Course.ApplicationUser.Id == userId,includeProperties:"Course")
        .Select(f => new
        {
            feedBackId = f.FeedBackId,
            feedBackStar = f.FeedBackStar,  
            feedBackContent = f.FeedBackContent,  
            courseId = f.CourseId,  
            createdAt = f.CreatedAt,  
            courseName = f.Course.CourseName  
        })
        .ToList();

            return Json(new { data = feedback});
        }
    }
}
