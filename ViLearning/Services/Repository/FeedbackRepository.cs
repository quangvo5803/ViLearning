using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class FeedbackRepository : Repository<Feedback>,IFeedbackRepository
    {
        private ApplicationDBContext _db;
        public FeedbackRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Feedback feedback)
        {
            _db.Feedbacks.Update(feedback);
        }
    }
}
