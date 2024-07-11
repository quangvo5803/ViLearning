using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ManagerTeacherWithdrawController : Controller
    {

        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnPayServicecs _vpnPayServicecs;

        public ManagerTeacherWithdrawController(IUnitOfWork unitOfWork, IVnPayServicecs vnPayServicecs)
        {
            _unitOfWork = unitOfWork;
            _vpnPayServicecs = vnPayServicecs;
            
        }
        public IActionResult Index()
        {
            List<WithdrawRequest> objList = _unitOfWork.WithdrawRequest.GetRange(r => r.Status == false).ToList();

            return View(objList);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<WithdrawRequest> objList = _unitOfWork.WithdrawRequest.GetRange(r => r.Status == false).ToList();
            foreach (WithdrawRequest obj in objList)
            {
                obj.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == obj.UserId);
            }
            return Json(new { data = objList });
        }

        [Authorize]
        public IActionResult Payment(int withdrawId)
        {
            var userName = User.Identity.Name;
            var withdraw = _unitOfWork.WithdrawRequest.Get(r => r.WithdrawRequestID == withdrawId);
            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = withdraw.RequestMoney,
                CreateDate = DateTime.Now,
                Description = "Xử lí yêu cầu rút tiền " + withdrawId,
                FullName = userName,
                OrderId = new Random().Next(1000, 100000),
                WithdrawRequest = withdraw
            };
            return Redirect(_vpnPayServicecs.CreatePaymentUrl(HttpContext, vnPayModel));
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vpnPayServicecs.PaymentExecute(Request.Query);
            var code = response.VnPayResponseCode;
            var withdrawId = response.OrderDescription;
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["error"] = "Lỗi thanh toán";
                return RedirectToAction("Index");

            }
            WithdrawRequest withdraw = _unitOfWork.WithdrawRequest.Get(r => r.WithdrawRequestID == int.Parse(withdrawId));
            withdraw.Status = true;
            withdraw.CompleteDay = DateTime.Now;
            _unitOfWork.WithdrawRequest.Update(withdraw);
            _unitOfWork.Save();
            TempData["success"] = "Thanh toán thành công";
            return RedirectToAction("Index");
        }
    }
}
