using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class CourseCertificateRepository : Repository<CourseCertificate>,ICourseCertificateRepository
    {
        private ApplicationDBContext _db;
        public CourseCertificateRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CourseCertificate courseCertificate)
        {
            _db.CourseCertificates.Update(courseCertificate);
        }
    }
}
