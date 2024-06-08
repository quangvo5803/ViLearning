using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = SD.Role_User_Teacher)]
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> CourseManage()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            List<Course> courseList = await _unitOfWork.Course.GetCourseByOwnerId(userId);
            foreach (var course in courseList)
            {
                course.Lesson = await _unitOfWork.Lesson.GetLessonByCourseId(course.CourseId);
            }

            return View(courseList);
        }
    }
}
