using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.DailyOrder.Request;
using PerfectBreakfast.Application.Models.DailyOrder.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IDailyOrderService
    {
        public Task<OperationResult<DailyOrderResponse>> CreateDailyOrder(DailyOrderRequest dailyOrderRequest);
        public Task<OperationResult<List<DailyOrderResponse>>> GetDailyOrders();
        public Task<OperationResult<Pagination<DailyOrderResponse>>> GetDailyOrderPaginationAsync(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<DailyOrderResponse>> UpdateDailyOrder(Guid id, UpdateDailyOrderRequest updateDailyOrderRequest);
        public Task<OperationResult<Pagination<DailyOrderForPartnerResponse>>> GetDailyOrderByPartner(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<Pagination<DailyOrderForDeliveryResponse>>> GetDailyOrderByDelivery(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<TotalFoodForCompanyResponse>> GetDailyOrderDetail(Guid id, DateOnly bookingDate);
    }
}
