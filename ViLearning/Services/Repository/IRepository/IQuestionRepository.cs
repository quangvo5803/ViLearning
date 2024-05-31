using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IQuestionRepository : IRepository<Question>
    {
        void Update (Question question);
    }
}
