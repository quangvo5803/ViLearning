using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Areas.Identity.Pages.Account.Manage
{
    public class BalanceTeacherModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public BalanceTeacherModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            Role = new List<string>();
        }

        public IList<string> Role { get; set; }
        public List<WithdrawRequest>? WithdrawRequests { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        public double Balance { get; set; }
        public class InputModel
        {
            [DisplayName("Nhập số dư muốn rút")]
            [Required(ErrorMessage = "Vui lòng nhập số dư muốn rút")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
            public double Withdraw { get; set; }          
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Balance = user.Balance;
            Role = await _userManager.GetRolesAsync(user);
            WithdrawRequests = (List<WithdrawRequest>)_unitOfWork.WithdrawRequest.GetRange(r => r.UserId == user.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (Input.Withdraw > user.Balance)
            {
                TempData["Error"] = "Số tiền hiện tại không đủ";
                return RedirectToPage("./BalanceTeacher");
            }
            if (Input.Withdraw <= 100000)
            {
                TempData["Error"] = "Vui lòng rút nhiều hơn 100.000 VNĐ";
                return RedirectToPage("./BalanceTeacher");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu ko hợp lệ";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
                return RedirectToPage("./BalanceTeacher");
            }
            WithdrawRequest withdrawRequest = new WithdrawRequest
            {
                UserId = user.Id,
                RequestMoney = Input.Withdraw,
                RequestDay = DateTime.Now,
                Status = false
            };
            user.Balance -= Input.Withdraw;
            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.WithdrawRequest.Add(withdrawRequest);
            _unitOfWork.Save();
            StatusMessage = "Yêu cầu rút tiền thành công";
            TempData["Success"] = "Yêu cầu rút tiền thành công";
            return RedirectToPage("./BalanceTeacher");
        }
    }
}

