using System.Linq.Expressions;
using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class SubjectRepository : Repository<Subject>,ISubjectRepository
    {
        private ApplicationDBContext _db;
        public SubjectRepository(ApplicationDBContext db):base(db) 
        { 
            _db = db;
        }

        public void Update(Subject subject)
        {
            _db.Subjects.Update(subject);
        }
    }
}
