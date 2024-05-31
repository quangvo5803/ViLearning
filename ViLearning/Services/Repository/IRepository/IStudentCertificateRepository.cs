using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IStudentCertificateRepository : IRepository<StudentCertificate>
    {
        void Update(StudentCertificate studentCertificate);
    }
}
