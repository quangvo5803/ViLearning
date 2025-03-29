using ViLearning.Services.Repository;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Services.Services;
using ViLearning.Services.Services.IServices;

namespace ViLearning.Worker
{
    public class CertificateWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CertificateWorker> _logger;

        public CertificateWorker(IServiceScopeFactory serviceScopeFactory, ILogger<CertificateWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope()) // Create new scope
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    try
                    {
                        var completedCourses = unitOfWork.LearningProgress.GetRange(c => !c.IsEmailCert && c.Progress == 100, includeProperties: "User,Course");
                        if (completedCourses.Count() == 0) _logger.LogInformation("No course completion found!");
                        else
                        {
                            foreach (var course in completedCourses)
                            {
                                await emailService.SendCertificateEmailAsync(course);

                                course.IsEmailCert = true;
                            }

                            unitOfWork.Save();
                            _logger.LogInformation("Email sent successfully");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error when sending email: {ex.Message}");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Run every 10s
            }
        }
    }
}
