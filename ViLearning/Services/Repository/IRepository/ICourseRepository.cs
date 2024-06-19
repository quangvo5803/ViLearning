using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ICourseRepository : IRepository<Course>
    {
        void Update(Course course);
        public void LoadCourse(Course course);
        Task<List<Course>> GetCourseByOwnerId(string id);
        void Delete(int id);
    }
}
