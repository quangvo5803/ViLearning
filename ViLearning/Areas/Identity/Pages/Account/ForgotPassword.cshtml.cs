// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ViLearning.Models;

namespace ViLearning.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code, Input.Email },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
    Input.Email,
    "Reset Mật Khẩu",
    $"<table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;\">" +
    $"<tr>" +
    $"<td></td>" +
    $"<td class=\"container\" style=\"margin: 0 auto !important; max-width: 600px; padding: 0; padding-top: 24px; width: 600px;\">" +
    $"<div class=\"content\" style=\"box-sizing: border-box; display: block; margin: 0 auto; max-width: 600px; padding: 0;\">" +
    $" <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"main\" style=\"background: #ffffff; border: 1px solid #eaebed; border-radius: 16px; width: 100%; text-align: center;\">" +
    $"                <tr>" +
    $"                <td class=\"wrapper\" style=\"box-sizing: border-box; padding: 24px;\">\r\n" +
    $"                  <p style=\"font-weight: normal; margin: 0; margin-bottom: 16px; color:#000000;\">Chào bạn</p>\r\n" +
    $"                  <p style=\"font-weight: normal; margin: 0; margin-bottom: 16px; color:#000000;\">Chúng tôi cần xác thực email của bạn trước khi thay đổi mật khẩu. Vui lòng bấm vào nút bên dưới.</p>\r\n" +
    $"                  <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"btn btn-primary\" style=\"min-width: 100% !important; width: 100%;\">\r\n" +
    $"                    <tbody>\r\n" +
    $"                      <tr>\r\n" +
    $"                        <td align=\"center\" style=\"padding-bottom: 16px;\">\r\n" +
    $"                          <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">\r\n" +
    $"                            <tbody>\r\n" +
    $"                              <tr>\r\n" +
    $"                                <td><a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style=\"background-color: #ffffff; border: solid 2px #0867ec; border-radius: 4px; box-sizing: border-box; color: #0867ec; cursor: pointer; display: inline-block; font-size: 16px; font-weight: bold; margin: 0; padding: 12px 24px; text-decoration: none; text-transform: capitalize;\">Bấm vào đây</a></td>\r\n" +
    $"                              </tr>\r\n" +
    $"                            </tbody>\r\n" +
    $"                          </table>\r\n" +
    $"                        </td>\r\n" +
    $"                      </tr>\r\n" +
    $"                    </tbody>\r\n" +
    $"                  </table>\r\n" +
    $"                  <p style=\"font-weight: normal; margin: 0; margin-bottom: 16px;color:#000000;\">Cảm ơn bạn đã tin tưởng Vi-Learning</p>\r\n" +
    $"                  <p style=\"font-weight: normal; margin: 0; margin-bottom: 16px;color:#000000;\">Vi-Learning</p>\r\n" +
    $"                </td>\r\n" +
    $"              </tr>\r\n" +
    $"            </table>\r\n" +
    $"          </div>\r\n" +
    $"        </td>\r\n" +
    $"        <td></td>\r\n" +
    $"      </tr>\r\n" +
    $"    </table>\r\n");


                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }

}
