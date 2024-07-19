using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ILessonRepository :IRepository<Lesson>
    {
        Task<List<Lesson>?> GetLessonByCourseId(int courseId);
        void Update(Lesson lesson);
        public void LoadTest(Lesson lesson);

        public void UnDetachLesson(Lesson lesson);
    }
}
