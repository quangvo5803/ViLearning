using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ICourseCertificateRepository : IRepository<CourseCertificate>
    {
        void Update(CourseCertificate courseCertificate);
    }
}
