using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class WithdrawRequestRepositoy : Repository<WithdrawRequest>, IWithdrawRequestRepositoy
    {
        private ApplicationDBContext _db;
        public WithdrawRequestRepositoy(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WithdrawRequest request)
        {
            _db.WithdrawRequests.Update(request);
        }
    }
}
