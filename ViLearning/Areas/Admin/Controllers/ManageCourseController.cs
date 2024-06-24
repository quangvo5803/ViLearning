using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ManageCourseController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public ManageCourseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var pendingCourses = _unitOfWork.Course.GetAll().Where(c => c.CourseStatus == CourseStatus.Pending);
            var courseUserVMs = pendingCourses.Select(course => new CourseUserVM
            {
                Course = course,
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == course.UserId)
            }).ToList();

            return View(courseUserVMs);

        }

        [HttpPost]
        public IActionResult Review(int courseId, string action)
        {
            var course = _unitOfWork.Course.Get(c => c.CourseId == courseId, includeProperties: "ApplicationUser");

            if (course == null)
            {
                return NotFound("Khóa học không tồn tại.");
            }

            if (action == "Published")
            {
                course.CourseStatus = CourseStatus.Published;
                TempData["SuccessMessage"] = "Khóa học đã được phê duyệt và xuất bản thành công.";
            }
            else if (action == "Rejected")
            {
                course.CourseStatus = CourseStatus.Rejected;
                TempData["ErrorMessage"] = "Khóa học đã bị từ chối.";
            }

            _unitOfWork.Course.Update(course);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var course = _unitOfWork.Course.Get(c => c.CourseId == id, includeProperties: "ApplicationUser,Lessons");
            if (course == null)
            {
                return NotFound("Khóa học không tồn tại.");
            }

            return View(course);
        }


    }
}

