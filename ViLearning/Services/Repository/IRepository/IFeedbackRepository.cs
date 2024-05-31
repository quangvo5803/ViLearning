using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IFeedbackRepository : IRepository<Feedback>
    {
        void Update(Feedback feedback);
    }
}
