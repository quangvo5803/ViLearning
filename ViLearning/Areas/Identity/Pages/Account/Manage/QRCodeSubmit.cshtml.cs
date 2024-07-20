using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using ViLearning.Models;
using ViLearning.Utility;

namespace ViLearning.Areas.Identity.Pages.Account.Manage
{
    public class QRCodeSubmitModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly BlobStorageService _blobStorageService;

        public QRCodeSubmitModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            BlobStorageService blobStorageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _blobStorageService = blobStorageService;
            Role = new List<string>();
        }

        public IList<string> Role { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [DisplayName("QR thanh toán")]
            public string? TeacherQRCodeUrl { get; set; }

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Role = await _userManager.GetRolesAsync(user);
            Input = new InputModel
            {
                TeacherQRCodeUrl = user.TeacherQrCodeUrl
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
                return RedirectToPage("./QRCodeSubmit");
            }

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file tải lên";
                return RedirectToPage("./QRCodeSubmit");
            }

            try
            {
                string containerName = "banking-qrcode";
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Check and delete old file from Azure Blob Storage
                if (!string.IsNullOrEmpty(user.TeacherQrCodeUrl))
                {
                    Uri oldUri = new Uri(user.TeacherQrCodeUrl);
                    string oldFileName = Path.GetFileName(oldUri.LocalPath);
                    await _blobStorageService.DeleteFileAsync(containerName, oldFileName);
                }

                // Upload new file to Azure Blob Storage
                using (var stream = file.OpenReadStream())
                {
                    user.TeacherQrCodeUrl = await _blobStorageService.UploadFileAsync(containerName, fileName, stream);
                }
                user.TeacherQrCode = true;
                // Update user in database
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    TempData["Error"] = "Xảy ra lỗi khi tải ảnh lên";
                    return RedirectToPage();
                }
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Thay đổi QR Code thành công";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi tải file,vui lòng tải file ảnh";
                return RedirectToPage("./CertificateSubmit");
            }
        }
    }
}
