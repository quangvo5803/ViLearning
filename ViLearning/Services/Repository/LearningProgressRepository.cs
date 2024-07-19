using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class LearningProgressRepository : Repository<LearningProgress>, ILearningProgressRepository
    {
        private ApplicationDBContext _db;
        public LearningProgressRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void LoadCourse(LearningProgress learningProgress)
        {
            _db.LearningProgresses.Entry(learningProgress)
                .Reference(l => l.Course)
                .Load();
        }

        public void Update(LearningProgress learningProgress)
        {
            _db.LearningProgresses.Update(learningProgress);
        }
    }
}
