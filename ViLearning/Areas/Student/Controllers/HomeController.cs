using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll().ToList();
            List<ApplicationUser> teacherList = new List<ApplicationUser>();
            foreach (ApplicationUser user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Teacher"))
                {
                    teacherList.Add(user);
                }
            }
            var viewModel = new LandingPageVM
            {
                Courses = _unitOfWork.Course.GetAll(includeProperties: "Subject,ApplicationUser").ToList(),
                UserList = userList,
                TeacherList = teacherList
            };
            return View(viewModel);
        }

        public IActionResult Details(int CourseId)
        {
            var lessonOfCourse = _unitOfWork.Lesson.GetRange(c=>c.Course.CourseId == CourseId,includeProperties:"Course");
            List<Lesson> lessons = _unitOfWork.Lesson.GetAll().ToList();
            Course course = _unitOfWork.Course.Get(c => c.CourseId == CourseId, includeProperties: "Subject,ApplicationUser");
            var detailViewModel = new CourseDetailsVM
            {
                Course = course,
                Lessons = lessonOfCourse
            };
            return View(detailViewModel);
        }
        
        public async Task<IActionResult> CourseListAsync()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll().ToList();
            List<ApplicationUser> teacherList = new List<ApplicationUser>();
            foreach (ApplicationUser user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Teacher"))
                {
                    teacherList.Add(user);
                }
            }
            var viewModel = new LandingPageVM
            {
                Courses = _unitOfWork.Course.GetAll(includeProperties: "Subject,ApplicationUser").ToList(),
                UserList = userList,
                TeacherList = teacherList
            };
            return View(viewModel);
        }

        public IActionResult Search(string query) 
        {
            List<ApplicationUser> teacherList = new List<ApplicationUser>();
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll().ToList();
            foreach (ApplicationUser user in userList)
            {
                if (user.Role == "Teacher")
                {
                    teacherList.Add(user);
                }
            }
            var viewModel = new LandingPageVM
            {
                Courses = _unitOfWork.Course.GetRange(c => c.CourseName.Contains(query)
                                                            || c.Subject.Name.Contains(query)
                                                            || c.Description.Contains(query)
                                                            || c.ApplicationUser.FullName.Contains(query), includeProperties: "Subject,ApplicationUser"),
                UserList = userList,
                TeacherList = teacherList
            };
            return View(viewModel);
        }

        [Authorize]
        public IActionResult Checkout(int CourseId)
        { 
            var course = _unitOfWork.Course.Get(c=>c.CourseId==CourseId);
            var lessonOfCourse = _unitOfWork.Lesson.GetRange(c => c.Course.CourseId == CourseId, includeProperties: "Course");
            var detailViewModel = new CourseDetailsVM
            {
                Course = course,
                Lessons = lessonOfCourse
            };
            return View(detailViewModel);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
