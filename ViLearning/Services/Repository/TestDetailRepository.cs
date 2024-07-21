using Microsoft.EntityFrameworkCore;
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

        public async Task<double> GetHighestMarkByLessonIdAsync(int lessonId, string userId)
        {
            return await _db.Set<TestDetail>()
            .Where(td => td.LessonId == lessonId && td.UserId == userId)
            .OrderByDescending(td => td.Mark)
            .Select(td => td.Mark)
            .FirstOrDefaultAsync();
        }

        public void Update(TestDetail testDetail)
        {
            _db.TestDetails.Update(testDetail);
        }
    }
}
