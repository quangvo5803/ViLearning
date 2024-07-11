using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        void Update(Conversation conversation);
    }
}
