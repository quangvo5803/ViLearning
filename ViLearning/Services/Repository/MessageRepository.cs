using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class MessageRepository : Repository<Message>,IMessageRepository
    {
        private ApplicationDBContext _db;
        public MessageRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Message message)
        {
            _db.Messages.Update(message);
        }
    }
}
