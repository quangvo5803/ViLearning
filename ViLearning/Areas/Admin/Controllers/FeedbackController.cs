using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            var feedback = _unitOfWork.Feedback.GetAll(includeProperties: "Course").ToList();
            return View(feedback);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = _userManager.GetUserId(User);
            var feedback = _unitOfWork.Feedback
        .GetAll(includeProperties: "Course")
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

            return Json(new { data = feedback });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Feedback feedback = _unitOfWork.Feedback.Get(f => f.FeedBackId == id);
            if (feedback == null)
            {
                return Json(new { success = false, message = "Có lỗi khi xóa" });
            }
            _unitOfWork.Feedback.Remove(feedback);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });
        }
    }
}
