using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IVnPayServicecs
    {
        string CreatePaymentUrl(HttpContext context,VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
