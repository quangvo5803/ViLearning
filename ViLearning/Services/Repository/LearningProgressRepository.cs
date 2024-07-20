using Microsoft.AspNetCore.Routing.Template;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;
using Microsoft.IdentityModel.Tokens;

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


        public bool HasLearnedLesson(string learnedLessons, int lessonNo)
        {
            if (string.IsNullOrEmpty(learnedLessons))
            {
                return false;
            }

            string[] lessonsArray = learnedLessons.Split(',');
            return lessonsArray.Contains(lessonNo.ToString());
        }

        public bool HasLearnedLesson(LearningProgress learningProgress, int lessonNo)
        {
            if (string.IsNullOrEmpty(learningProgress.LearnedLessons))
            {
                return false;
            }

            string[] lessonsArray = learningProgress.LearnedLessons.Split(',');
            return lessonsArray.Contains(lessonNo.ToString());
        }

        public int NumOfLessonLearned(LearningProgress learningProgress)
        {
            if (string.IsNullOrEmpty(learningProgress.LearnedLessons))
            {
                return 0;
            }

            string[] lessonsArray = learningProgress.LearnedLessons.Split(',');
            return lessonsArray.Length;
        }
    }
}
