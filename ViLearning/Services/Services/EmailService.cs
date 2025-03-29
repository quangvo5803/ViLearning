using MailKit.Net.Smtp;
using MimeKit;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Services.Services.IServices;

namespace ViLearning.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task SendCertificateEmailAsync(LearningProgress learningProgress)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ViLearning", emailSettings["SmtpUser"]));
            message.To.Add(new MailboxAddress(learningProgress.User.FullName, learningProgress.User.Email));
            message.Subject = $"🎓 CERTIFICATE FOR COURSE COMPLETION: {learningProgress.Course.CourseName}";

            var body = new TextPart("plain")
            {
                Text = $"Congratulation to {learningProgress.User.FullName}!\n\n" +
                       $"You have finished '{learningProgress.Course.CourseName}' on ViLearning.\n" +
                       $"Your certificate is attached in this email.\n\n" +
                       $"Thanks you for choosing our platform!"
            };

            MimePart attachment;
            using (HttpClient httpClient = new HttpClient())
            {
                byte[] fileBytes = await httpClient.GetByteArrayAsync(learningProgress.StudentCertificateUrl);
                var stream = new MemoryStream(fileBytes);

                attachment = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(stream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = $"{learningProgress.Course.CourseName}_Certificate.pdf"
                };
            }

            var multipart = new Multipart("mixed") { body, attachment };
            message.Body = multipart;

            using var client = new SmtpClient();
            await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), false);
            await client.AuthenticateAsync(emailSettings["SmtpUser"], emailSettings["SmtpPass"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
