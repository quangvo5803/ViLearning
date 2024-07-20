using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IWithdrawRequestRepositoy : IRepository<WithdrawRequest>
    {
        void Update(WithdrawRequest request);
    }
}
