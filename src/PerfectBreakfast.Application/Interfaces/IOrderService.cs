using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Models.OrderModel.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OperationResult<OrderResponse>> CreateOrder(OrderRequest orderRequest);
        Task<OperationResult<List<OrderResponse>>> GetOrders();
        Task<OperationResult<OrderResponse>> GetOrder(Guid id);
        Task<OperationResult<OrderResponse>> UpdateOrder(Guid id, UpdateOrderRequest updateOrderRequest);
        Task<OperationResult<OrderResponse>> DeleteOrder(Guid id);
        public Task<OperationResult<Pagination<OrderResponse>>> GetOrderPaginationAsync(int pageIndex = 0, int pageSize = 10);
    }
}
