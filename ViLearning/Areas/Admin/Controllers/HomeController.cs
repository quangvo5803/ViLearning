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
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCounts()
        {
            var users = _unitOfWork.ApplicationUser.GetAll().ToList();
            var studentCount = 0;
            var teacherCount = 0;

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(SD.Role_User_Student))
                {
                    studentCount++;
                }
                else if (roles.Contains(SD.Role_User_Teacher))
                {
                    teacherCount++;
                }
            }

            var result = new
            {
                Students = studentCount,
                Teachers = teacherCount
            };

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyProfit(int year)
        {
            var invoiceList = _unitOfWork.Invoice.GetRange(i => i.PurchaseDate.Year == year).ToList();
            var monthlyProfit = new double[12];
            var monthlyRevenue = new double[12];

            foreach (var invoice in invoiceList)
            {
                var month = invoice.PurchaseDate.Month - 1; // Assuming invoice.Date is a DateTime object
                monthlyProfit[month] += invoice.Amount * 0.1;
                monthlyRevenue[month] += invoice.Amount;
            }

            var result = new
            {
                Months = new[] { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" },
                Profits = monthlyProfit,
                Revenues = monthlyRevenue
            };

            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetCourse()
        {
            var pendingCourseList = _unitOfWork.Course.GetRange(c => c.CourseStatus == CourseStatus.Pending).ToList();
            var approvedCourseList = _unitOfWork.Course.GetRange(c => c.CourseStatus == CourseStatus.Published).ToList();

            var result = new
            {
                ApprovedCourse = approvedCourseList.Count(),
                PendingCourse = pendingCourseList.Count()
            };

            return Json(result);
        }
    }
}