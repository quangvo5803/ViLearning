using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ITestDetailRepository : IRepository<TestDetail>
    {
        void Update (TestDetail testDetail);
        Task<double> GetHighestMarkByLessonIdAsync(int lessonId, string userId);
    }
}
