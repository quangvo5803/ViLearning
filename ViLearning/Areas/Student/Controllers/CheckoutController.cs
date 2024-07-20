using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Models;
using ViLearning.Models.ViewModels;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Areas.Student.Controllers
{
    [Area("Student")]
    public class CheckoutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnPayServicecs _vpnPayServicecs;
        public CheckoutController(IUnitOfWork unitOfWork, IVnPayServicecs vnPayServicecs)
        {
            _unitOfWork = unitOfWork;
            _vpnPayServicecs = vnPayServicecs;
        }
        [Authorize]
        public IActionResult Checkout(int CourseId)
        {

            var course = _unitOfWork.Course.Get(c => c.CourseId == CourseId);
            var lessonOfCourse = _unitOfWork.Lesson.GetRange(c => c.Course.CourseId == CourseId, includeProperties: "Course");
            var detailViewModel = new CourseDetailsVM
            {
                Course = course,
                Lessons = lessonOfCourse
            };
            return View(detailViewModel);
        }
        [Authorize]
        public IActionResult Payment(int CourseId)
        {
            var course = _unitOfWork.Course.Get(c => c.CourseId == CourseId);
            var userName = User.Identity.Name;
            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = (double)course.Price,
                CreateDate = DateTime.Now,
                Description = "Thanh toán đơn hàng",
                FullName = userName,
                OrderId = new Random().Next(1000, 100000),
                Course = course
            };
            return Redirect(_vpnPayServicecs.CreatePaymentUrl(HttpContext, vnPayModel));
        }
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            //Create payment
            var response = _vpnPayServicecs.PaymentExecute(Request.Query);
            var code = response.VnPayResponseCode;
            var courseId = response.OrderDescription;
            var course = _unitOfWork.Course.Get(c=> c.CourseId == int.Parse(courseId), includeProperties: "Subject,ApplicationUser");
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["error"] = "Lỗi thanh toán";
                return RedirectToAction("Details", "Home", new { CourseId = courseId });

            }
            //Create invoice if payment success
            Invoice invoice = new Invoice();
            invoice.CourseId = int.Parse(courseId);
            invoice.Amount = (double)course.Price;
            invoice.PurchaseDate = DateTime.Now;
            invoice.UserId = User.Identity.GetUserId();
            _unitOfWork.Invoice.Add(invoice);
            //Add money for teacher
           var user = _unitOfWork.ApplicationUser.Get(u => u.Id == course.ApplicationUser.Id);
            user.Balance += (double)course.Price * 0.9;
            _unitOfWork.Save();
            TempData["success"] = "Thanh toán thành công";
            return RedirectToAction("Details", "Home", new { CourseId = courseId });
        }
    }
}
