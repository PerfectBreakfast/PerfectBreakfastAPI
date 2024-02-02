using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        public Task<Order> GetOrderByOrderCode(int orderCode);
        public Task<List<Order>> GetOrderByDailyOrderId(Guid dailyOrderId);
        public Task<List<Order>> GetOrderHistory(Guid userId, params IncludeInfo<Order>[] includeProperties);
    }
}
