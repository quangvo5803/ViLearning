using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = SD.Role_User_Student)]
    public class LearningController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LearningController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int courseId, int lessonNo)
        {
            var course = _unitOfWork.Course.Get( c => c.CourseId == courseId );
            List<Lesson>? lessons = await _unitOfWork.Lesson.GetLessonByCourseId(courseId);
            var lesson = lessons?.Where(l => l.LessonNo == lessonNo).FirstOrDefault();
            LearningMaterial lm = new LearningMaterial()
            {
                Course = course,
                Lesson = lesson
            };
            return View(lm);
        }
    }
}
