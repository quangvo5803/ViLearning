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
    public class ManagerTeacherWithdrawController : Controller
    {

        
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ManagerTeacherWithdrawController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;           
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
        public IActionResult Withdraw(int id)
        {
            var request = _unitOfWork.WithdrawRequest.Get(r => r.WithdrawRequestID == id);
            foreach(ApplicationUser u in _unitOfWork.ApplicationUser.GetAll())
            {
                if(request.UserId == u.Id)
                {
                    request.ApplicationUser = u;
                }
            }
            return View(request);
        }

        public IActionResult Accept(int id)
        {
            var request = _unitOfWork.WithdrawRequest.Get(r => r.WithdrawRequestID == id);
            request.Status = true;
            _unitOfWork.WithdrawRequest.Update(request);
            _unitOfWork.Save();
            return RedirectToAction("Index", "ManagerTeacherWithdraw");
        }

        public IActionResult Reject(int id)
        {
            var request = _unitOfWork.WithdrawRequest.Get(r => r.WithdrawRequestID == id);
            foreach (ApplicationUser u in _unitOfWork.ApplicationUser.GetAll())
            {
                if (request.UserId == u.Id)
                {
                    request.ApplicationUser = u;
                }
            }
            request.ApplicationUser.Balance += request.RequestMoney;
            _unitOfWork.ApplicationUser.Update(request.ApplicationUser);
            _unitOfWork.WithdrawRequest.Remove(request);
            _unitOfWork.Save();
            return RedirectToAction("Index", "ManagerTeacherWithdraw");
        }

    }
}
