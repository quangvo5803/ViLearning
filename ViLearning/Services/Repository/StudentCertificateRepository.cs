using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class StudentCertificateRepository : Repository<StudentCertificate>,IStudentCertificateRepository
    {
        private ApplicationDBContext _db;
        public StudentCertificateRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(StudentCertificate studentCertificate)
        {
            _db.StudentCertificates.Update(studentCertificate);
        }
    }
}
