using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ViLearning.Models;

namespace ViLearning.Areas.Identity.Pages.Account.Manage
{
    public class CertificateSubmitModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CertificateSubmitModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [DisplayName("Chứng chỉ giáo viên")]
            public string? TeacherCertificateImgUrl { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input = new InputModel
            {
                TeacherCertificateImgUrl = user.TeacherCertificateImgUrl
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                StatusMessage = "Model state is invalid.";
                return Page();
            }

            if (file == null || file.Length == 0)
            {
                StatusMessage = "No file selected or file is empty.";
                return Page();
            }

            try
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string certificatePath = Path.Combine(wwwRootPath, "images", "CertificateTeacher");

                //Kiểm tra thư mục có tồn tại chưa chưa thì tạo
                if (!Directory.Exists(certificatePath))
                {
                    Directory.CreateDirectory(certificatePath);
                }

                //Kiểm tra file cũ nếu có thì xóa
                if (!string.IsNullOrEmpty(user.TeacherCertificateImgUrl))
                {
                    var oldCertificate = Path.Combine(wwwRootPath, user.TeacherCertificateImgUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldCertificate))
                    {
                        System.IO.File.Delete(oldCertificate);
                    }
                }

                //Tạo đường đẫn file mới và copy vào thư mục 
                string filePath = Path.Combine(certificatePath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                //Thay đổi cập nhật lại database
                user.TeacherCertificateImgUrl = Path.Combine("images", "CertificateTeacher", fileName).Replace("\\", "/");
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to update teacher certificate status.";
                    return RedirectToPage();
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Nộp chứng chỉ thành công vui lòng đợi duyệt";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error uploading file: {ex.Message}";
                return Page();
            }
        }
    }
}
