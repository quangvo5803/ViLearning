using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class TestDetailRepository : Repository<TestDetail>, ITestDetailRepository
    {
        private ApplicationDBContext _db;
        public TestDetailRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(TestDetail testDetail)
        {
            _db.TestDetails.Update(testDetail);
        }
    }
}
