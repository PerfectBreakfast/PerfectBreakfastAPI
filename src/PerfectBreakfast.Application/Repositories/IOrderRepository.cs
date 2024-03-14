using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        public Task<Order> GetOrderByOrderCode(long orderCode);
        public Task<List<Order>> GetOrderByDailyOrderId(Guid dailyOrderId);
        public Task<List<Order>> GetOrderHistory(Guid userId,int pageNumber, params IncludeInfo<Order>[] includeProperties);
        public Task<List<Order>> GetOrderByDate(DateOnly fromDate, DateOnly toDate);
    }
}
