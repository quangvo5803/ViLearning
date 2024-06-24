﻿using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace ViLearning.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];

            try
            {
                using (var message = new MailMessage())
                using (var smtpClient = new SmtpClient())
                {
                    message.From = new MailAddress(smtpUser, "ViLearning");
                    message.To.Add(email);
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.Body = htmlMessage;

                    smtpClient.Port = 587;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await smtpClient.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email sending failed: {ex.Message}", ex);
                throw new InvalidOperationException($"Email sending failed: {ex.Message}", ex);
            }
        }
    }
}
