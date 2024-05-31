using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class ContentRepository : Repository<Content>, IContentRepository
    {
        private ApplicationDBContext _db;
        public ContentRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Content content)
        {
            _db.Contents.Update(content);
        }
    }
}
