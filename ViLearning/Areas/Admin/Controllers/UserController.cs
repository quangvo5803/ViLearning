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
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<ApplicationUser> objCategoryList = _unitOfWork.ApplicationUser.GetAll().ToList();

            return View(objCategoryList);
        }

        public IActionResult Submit(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            return View(user);
        }

        public async Task<IActionResult> Accept(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            if (user != null)
            {
                user.TeacherCertificate = true;
                var removeFromRoleResult = await _userManager.RemoveFromRoleAsync(user, SD.Role_User_Student);

                if (removeFromRoleResult.Succeeded)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, SD.Role_User_Teacher);

                    if (addToRoleResult.Succeeded)
                    {
                        _unitOfWork.Save();
                        TempData["success"] = "Duyệt giáo viên thành công";
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        // Log or handle the error if adding role fails
                        TempData["error"] = "Không thể thêm vai trò giáo viên cho người dùng";
                    }
                }
                else
                {
                    // Log or handle the error if removing role fails
                    TempData["error"] = "Không thể xóa vai trò học sinh của người dùng";
                }
            }
            else
            {
                // Log or handle the error if user is not found
                TempData["error"] = "Người dùng không tồn tại";
            }

            return RedirectToAction("Index", "User");
        }

        public IActionResult Reject(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while delete" });
            }
            var cerfiticateUrl = Path.Combine(_webHostEnvironment.WebRootPath, user.TeacherCertificateImgUrl.TrimStart('\\'));
            if (System.IO.File.Exists(cerfiticateUrl))
            {
                System.IO.File.Delete(cerfiticateUrl);
            }
            user.TeacherCertificateImgUrl = null;
            _unitOfWork.Save();
            TempData["success"] = "Đã từ chối đơn xét duyệt";
            return RedirectToAction("Index", "User");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll().ToList();
            foreach(var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            } 
            return Json(new { data = objUserList });
        }
        #endregion
    }
}
