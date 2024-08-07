﻿using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ViLearning.Models;
using ViLearning.Utility;

namespace ViLearning.Areas.Identity.Pages.Account.Manage
{
    public class CertificateSubmitModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly BlobStorageService _blobStorageService;

        public CertificateSubmitModel(
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
            Role = await _userManager.GetRolesAsync(user);
            Input = new InputModel
            {
                TeacherCertificateImgUrl = user.TeacherCertificateImgUrl,              
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
                return RedirectToPage("./CertificateSubmit");
            }

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "No file selected or file is empty.";
                return RedirectToPage("./CertificateSubmit");
            }

            try
            {
                string containerName = "teacher-certificate";
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Check and delete old file from Azure Blob Storage
                if (!string.IsNullOrEmpty(user.TeacherCertificateImgUrl))
                {
                    Uri oldUri = new Uri(user.TeacherCertificateImgUrl);
                    string oldFileName = Path.GetFileName(oldUri.LocalPath);
                    await _blobStorageService.DeleteFileAsync(containerName, oldFileName);
                }

                // Upload new file to Azure Blob Storage
                using (var stream = file.OpenReadStream())
                {
                    user.TeacherCertificateImgUrl = await _blobStorageService.UploadFileAsync(containerName, fileName, stream);
                }

                // Update user in database
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    TempData["Error"] = "Unexpected error when trying to update teacher certificate status.";
                    return RedirectToPage();
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Nộp chứng chỉ thành công vui lòng đợi duyệt";
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
