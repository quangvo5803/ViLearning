using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TestController testController = new TestController(_unitOfWork);
            var course = _unitOfWork.Course.Get( c => c.CourseId == courseId );
            List<Lesson>? lessons = await _unitOfWork.Lesson.GetLessonByCourseId(courseId);
            var lesson = lessons?.Where(l => l.LessonNo == lessonNo).FirstOrDefault();
            var lessonId = _unitOfWork.Lesson.Get(l => l.CourseId == courseId && l.LessonNo == lessonNo).LessonId;
            var tests = _unitOfWork.TestDetail.GetRange(t => t.LessonId == lessonId);
            foreach(Lesson l in lessons)
            {
                _unitOfWork.Lesson.LoadTest(l);
            }
            LearningMaterial lm = new LearningMaterial()
            {
                Course = course,
                Lesson = lesson,
                ListLesson = lessons,
                TestHistory = testController.TestHistory(lessonId, userId),
                TestRanking = testController.TestRanking(lessonId)
            };
            return View(lm);
        }
    }
}
