using Microsoft.EntityFrameworkCore;
using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private ApplicationDBContext _db;
        public CourseRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Course course)
        {
            _db.Courses.Update(course);
        }
        public void LoadCourse(Course course)
        {
            _db.Courses.Entry(course)
                    .Collection(c => c.Lesson)
                    .Load();
            _db.Update(course);
        }

        public async Task<List<Course>> GetCourseByOwnerId(string id)
        {
            try
            {
                return await _db.Courses.Where(x => x.UserId == id).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void Delete(int id)
        {
            var course = Get(c => c.CourseId == id);
            if (course != null)
            {
                var lessons = _db.Lessons.Where(l => l.CourseId == id);
                _db.Lessons.RemoveRange(lessons);
                _db.Courses.Remove(course);
            }
        }

        public void LoadTeacher(Course course)
        {
            _db.Entry(course)
                .Reference(c => c.ApplicationUser)
                .Load();
        }
    }
}
