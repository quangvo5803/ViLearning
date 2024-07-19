using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ILearningProgressRepository : IRepository<LearningProgress>
    {
        void Update (LearningProgress learningProgress);
    }
}
