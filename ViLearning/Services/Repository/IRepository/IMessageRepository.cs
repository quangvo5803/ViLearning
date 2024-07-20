using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IMessageRepository : IRepository<Message>
    {
        void Update(Message message);
    }
}
