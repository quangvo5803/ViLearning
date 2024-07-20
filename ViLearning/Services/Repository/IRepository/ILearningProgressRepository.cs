using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ILearningProgressRepository : IRepository<LearningProgress>
    {
        void Update (LearningProgress learningProgress);
        void LoadCourse(LearningProgress learningProgress);

        public bool HasLearnedLesson(LearningProgress learningProgress, int lessonNo);

        public int NumOfLessonLearned(LearningProgress learningProgress);
    }
}
