using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ILessonRepository :IRepository<Lesson>
    {
        void Update(Lesson lesson);
    }
}
