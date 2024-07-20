
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = SD.Role_User_Teacher)]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            double MonthlyProfit = 0;
            double YearProfit = 0;
            double FeedbackStar = 0;
            var monthCourse = _unitOfWork.Invoice.GetRange(i => i.Course.ApplicationUser.Id == userId && i.PurchaseDate.Month == DateTime.Now.Month);
            var yearCourse = _unitOfWork.Invoice.GetRange(i => i.Course.ApplicationUser.Id == userId && i.PurchaseDate.Year == DateTime.Now.Year);
            foreach(Invoice i in monthCourse)
            {
                MonthlyProfit += i.Amount;
            }
            foreach (Invoice i in yearCourse)
            {
                YearProfit += i.Amount;
            }

            var feedback = _unitOfWork.Feedback.GetRange(f => f.Course.ApplicationUser.Id == userId).ToList();
            foreach(Feedback f in feedback)
            {
                FeedbackStar += f.FeedBackStar;
            }

            var statistics = new StaticsViewModel
            {
                MonthlyProfit = MonthlyProfit * 0.9,
                YearProfit = YearProfit * 0.9,
                FeedbackStar = FeedbackStar / feedback.Count()
            };
            return View(statistics);
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyProfit(int year)
        {
            var userId = _userManager.GetUserId(User);
            var invoiceList = _unitOfWork.Invoice.GetRange(i => i.PurchaseDate.Year == year && i.Course.UserId == userId).ToList();
            var monthlyProfit = new double[12];

            foreach (var invoice in invoiceList)
            {
                var month = invoice.PurchaseDate.Month - 1; 
                monthlyProfit[month] += invoice.Amount * 0.9;
            }

            var result = new
            {
                Months = new[] { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" },
                Profits = monthlyProfit,
            };

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetCourse()
        {
            var userId = _userManager.GetUserId(User);
            var pendingCourseList = _unitOfWork.Course.GetRange(c => c.CourseStatus == CourseStatus.Pending && c.ApplicationUser.Id == userId).ToList();
            var approvedCourseList = _unitOfWork.Course.GetRange(c => c.CourseStatus == CourseStatus.Published && c.ApplicationUser.Id == userId).ToList();

            var result = new
            {
                ApprovedCourse = approvedCourseList.Count(),
                PendingCourse = pendingCourseList.Count()
            };

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userId = _userManager.GetUserId(User);
            var count = _unitOfWork.Invoice.GetRange(i => i.Course.ApplicationUser.Id == userId).Count();
            var result = new
            {
                User = _unitOfWork.Invoice.GetRange(i => i.Course.ApplicationUser.Id == userId).Count()
            };
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFeedback()
        {
            var userId = _userManager.GetUserId(User);
            var feedback = _unitOfWork.Feedback.GetRange(f => f.Course.ApplicationUser.Id == userId).ToList();
            var result = new
            {
                Five = feedback.Where(f => f.FeedBackStar == 5).Count(),
                Four = feedback.Where(f => f.FeedBackStar == 4).Count(),
                Three = feedback.Where(f => f.FeedBackStar == 3).Count(),
                Two = feedback.Where(f => f.FeedBackStar == 2).Count(),
                One = feedback.Where(f => f.FeedBackStar == 1).Count()
            };
            return Json(result);
        }

    }
}
