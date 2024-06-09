using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<List<Question>> GetQuestionByLessonId(int lessonId);
        void Update (Question question);
    }
}
