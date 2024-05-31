using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IContentRepository :IRepository<Content>
    {
        void Update (Content content);
    }
}
