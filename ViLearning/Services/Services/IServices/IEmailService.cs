using ViLearning.Models;

namespace ViLearning.Services.Services.IServices
{
    public interface IEmailService
    {
        Task SendCertificateEmailAsync(LearningProgress learningProgress);
    }
}
