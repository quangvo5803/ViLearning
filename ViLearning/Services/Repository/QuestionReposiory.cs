using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private ApplicationDBContext _db;
        public QuestionRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Question question)
        {
            _db.Questions.Update(question);
        }
    }
}
