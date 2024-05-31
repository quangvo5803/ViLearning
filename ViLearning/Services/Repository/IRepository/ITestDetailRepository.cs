using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ITestDetailRepository : IRepository<TestDetail>
    {
        void Update (TestDetail testDetail);
    }
}
