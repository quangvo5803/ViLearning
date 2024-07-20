using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class ConversationRepository : Repository<Conversation>, IConversationRepository
    {
        private ApplicationDBContext _db;
        public ConversationRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Conversation conversation)
        {
            _db.Conversations.Update(conversation);
        }
    }
}
