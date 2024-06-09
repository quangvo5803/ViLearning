using Microsoft.EntityFrameworkCore;
using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        private ApplicationDBContext _db;
        public LessonRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
        
        public void Update(Lesson lesson)
        {
            _db.Lessons.Update(lesson);
        }

        public async Task<List<Lesson>?> GetLessonByCourseId(int courseId)
        {
            try
            {
                return await _db.Lessons.Where(x => x.CourseId == courseId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
