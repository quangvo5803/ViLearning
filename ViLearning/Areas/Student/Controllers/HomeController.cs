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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<ApplicationUser> teacherList = new List<ApplicationUser>();
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll().ToList();
            foreach (ApplicationUser user in userList)
            {
                if(user.Role == "Teacher")
                {
                    teacherList.Add(user);
                }
            }
            var viewModel = new LandingPageVM
            {
                Courses = _unitOfWork.Course.GetAll().ToList(),
                UserList = userList,
                TeacherList = teacherList
            };
            return View(viewModel);
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
