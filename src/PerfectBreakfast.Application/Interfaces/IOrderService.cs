using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OperationResult<PaymentResponse>> CreateOrder(OrderRequest orderRequest);
        Task<OperationResult<PaymentResponse>> GetLinkContinuePayment(Guid id);
        Task<OperationResult<PaymentResponse>> CancelOrder(long orderCode);
        Task<OperationResult<List<OrderResponse>>> GetOrders();
        Task<OperationResult<OrderResponse>> GetOrder(Guid id);
        Task<OperationResult<OrderResponse>> UpdateOrder(Guid id, UpdateOrderRequest updateOrderRequest);
        Task<OperationResult<OrderResponse>> DeleteOrder(Guid id);
        public Task<OperationResult<Pagination<OrderResponse>>> GetOrderPaginationAsync(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<List<OrderHistoryResponse>>> GetOrderHistory(int pageNumber);
        public Task<OperationResult<List<OrderHistoryResponse>>> GetOrderHistoryByDeliveryStaff(int pageNumber);
        public Task<OperationResult<OrderResponse>> RemoveOrder(Guid id);
        public Task<OperationResult<bool>> CompleteOrder(Guid id);
        public Task<OperationResult<OrderStatisticResponse>> OrderStatistic(DateOnly fromDate, DateOnly toDate);
        Task<OperationResult<List<OrderResponse>>> GetOrderByDailyOrder(Guid dailyOrderId);
    }
}
