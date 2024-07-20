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
            List<Course> courseList = _unitOfWork.Course.GetRange(c => c.CourseStatus == CourseStatus.Published, includeProperties: "Subject,ApplicationUser,Feedbacks").ToList();
            foreach(Course c in courseList)
            {
                c.Feedbacks = _unitOfWork.Feedback.GetRange(f => f.CourseId == c.CourseId).ToList();
            }
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
                Courses = courseList,
                UserList = userList,
                TeacherList = teacherList,
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int CourseId,int page =1)
        {
            int pageSize = 5;
            var lessonOfCourse = _unitOfWork.Lesson.GetRange(c => c.Course.CourseId == CourseId, includeProperties: "Course");
            List<Lesson> lessons = _unitOfWork.Lesson.GetAll().ToList();
            Course course = _unitOfWork.Course.Get(c => c.CourseId == CourseId, includeProperties: "Subject,ApplicationUser,Feedbacks");
            course.Feedbacks = _unitOfWork.Feedback.GetRange(f => f.CourseId == CourseId,includeProperties:"ApplicationUser").ToList();

            int totalFeedbacks = course.Feedbacks.Count();
            int totalPages = (int)Math.Ceiling(totalFeedbacks / (double)pageSize);

            var pageFeedbacks = course.Feedbacks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var user = await _userManager.GetUserAsync(User);
            string userId = "";
            if (user != null)
            {
                userId = user.Id;
            }

            var detailViewModel = new CourseDetailsVM
            {
                Course = course,
                Lessons = lessonOfCourse,
                Invoice = _unitOfWork.Invoice.Get(i => i.UserId == userId && i.CourseId == CourseId),
                Feedback = _unitOfWork.Feedback.Get(f => f.UserId == userId && f.CourseId == CourseId),
                Feedbacks = new FeedbacksViewModel
                {
                    Feedbacks = pageFeedbacks,
                    TotalPages = totalPages,
                    CurrentPage = page
                }
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
                Courses = _unitOfWork.Course.GetRange(c => c.CourseStatus == CourseStatus.Published,includeProperties: "Subject,ApplicationUser,Feedbacks").ToList(),
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
                                                            || c.ApplicationUser.FullName.Contains(query) 
                                                            && c.CourseStatus == CourseStatus.Published, includeProperties: "Subject,ApplicationUser,Feedbacks"),
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

        [Authorize]
        public async Task<IActionResult> Study()
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = "";
            if (user != null)
            {
                userId = user.Id;
            }
            var invoice = _unitOfWork.Invoice.GetRange(i => i.UserId == userId, includeProperties: "Course,Course.ApplicationUser,Course.Subject,Course.Feedbacks");
            return View(invoice);
        }
        [Authorize]
        public IActionResult Feedback(int courseId)
        {
            var course = _unitOfWork.Course.Get(c => c.CourseId == courseId, includeProperties: "Subject,ApplicationUser");
            return View(course);
        }

        
        [HttpPost]
        public async Task<IActionResult> Feedback(int Rating,string Description,int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            Feedback f = new Feedback()
            {
                FeedBackContent = Description,
                FeedBackStar = Rating,
                UserId = user.Id,
                CourseId = courseId,
                CreatedAt = DateTime.Now
            };
            TempData["success"] = "Cảm ơn bạn đã đánh giá";
            _unitOfWork.Feedback.Add(f);
            _unitOfWork.Save();
            return RedirectToAction("Details", "Home", new { CourseId = courseId });
        }

        [HttpGet]
        public IActionResult GetFeedbacks(int courseId, int? starRating = null, int page = 1)
        {
            int pageSize = 5;
            var feedbacksQuery = _unitOfWork.Feedback.GetRange(f => f.CourseId == courseId, includeProperties: "ApplicationUser");

            if (starRating.HasValue)
            {
                feedbacksQuery = feedbacksQuery.Where(f => f.FeedBackStar == starRating.Value);
            }

            var feedbacks = feedbacksQuery.ToList();
            int totalFeedbacks = feedbacks.Count();
            int totalPages = (int)Math.Ceiling(totalFeedbacks / (double)pageSize);
            var feedbacksPaged = feedbacks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new FeedbacksViewModel
            {
                Feedbacks = feedbacksPaged,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return PartialView("_FeedbackPartial", viewModel);
        }


    }
}
