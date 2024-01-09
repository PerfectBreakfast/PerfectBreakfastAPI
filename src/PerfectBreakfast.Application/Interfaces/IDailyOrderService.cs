using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.DaliyOrder.Request;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IDailyOrderService
    {
        public Task<OperationResult<DailyOrderResponse>> CreateDailyOrder(DailyOrderRequest dailyOrderRequest);
        public Task<OperationResult<List<DailyOrderResponse>>> GetDailyOrders();
        public Task<OperationResult<Pagination<DailyOrderResponse>>> GetDailyOrderPaginationAsync(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<DailyOrderResponse>> UpdateDailyOrder(UpdateDailyOrderRequest updateDailyOrderRequest);
    }
}
