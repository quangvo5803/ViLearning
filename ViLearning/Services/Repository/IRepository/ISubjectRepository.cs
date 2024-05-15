using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        void Update(Subject subject);
    }
}
