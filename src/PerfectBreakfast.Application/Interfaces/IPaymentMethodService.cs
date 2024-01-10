using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Request;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IPaymentMethodService
    {
        public Task<OperationResult<List<PaymentMethodResponse>>> GetPaymentMethods();
        public Task<OperationResult<PaymentMethodResponse>> GetPaymentMethod(Guid id);
        public Task<OperationResult<PaymentMethodResponse>> CreatePaymentMethod(PaymentMethodRequest paymentMethodRequest);
        public Task<OperationResult<PaymentMethodResponse>> DeletePaymentMethod(Guid id);
        public Task<OperationResult<PaymentMethodResponse>> UpdatePaymentMethod(Guid id, PaymentMethodRequest paymentMethodRequest);
        public Task<OperationResult<Pagination<PaymentMethodResponse>>> GetPaymentMethodPaginationAsync(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<PaymentMethodResponse>> Delete(Guid id);
    }
}
