using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ICourseRepository : IRepository<Course>
    {
        void Update(Course course);
    }
}
