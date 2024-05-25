using ViLearning.Data;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ISubjectRepository Subject { get; private set; }
        public IApplicationUserRepository ApplicationUser{ get; private set; }


        private ApplicationDBContext _db;

        public UnitOfWork(ApplicationDBContext db)
        {
            _db = db;
            Subject = new SubjectRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
