using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        void Update(Invoice invoice);
    }
}
