using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace ViLearning.Utility
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();

                message.From = new MailAddress("binlun582003@gmail.com", "ViLearning");
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlMessage;

                smtpClient.Port = 587;
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("binlun582003@gmail.com", "ffyotqzccijrvckz");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the error)
                throw new InvalidOperationException($"Email sending failed: {ex.Message}", ex);
            }
        }
    }
}
