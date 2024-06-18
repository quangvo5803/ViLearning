using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class InvoiceRepository : Repository<Invoice>,IInvoiceRepository
    {
        private ApplicationDBContext _db;
        public InvoiceRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Invoice invoice)
        {
            _db.Invoices.Update(invoice);
        }
    }
}
