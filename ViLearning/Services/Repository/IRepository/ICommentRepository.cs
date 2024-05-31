using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface ICommentRepository : IRepository<Comment>
    {
        void Update (Comment comment);
    }
}
