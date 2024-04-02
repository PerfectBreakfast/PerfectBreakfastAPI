using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IShippingOrderRepository : IGenericRepository<ShippingOrder>
{
    public Task<List<ShippingOrder>> GetShippingOrderTodayByShipperIdAndDate(Guid shipperId,DateOnly date);
    public Task<ShippingOrder?> GetShippingOrderByDailyOrderId(Guid shipperId, Guid dailyOrderId);
    public Task<bool> ExistsWithDailyOrderAndShippers(Guid dailyOrderId, List<Guid?> shipperId);
    public Task<List<User?>> GetStaffByDailyOrder(Guid dailyOrderId);
    public Task<List<ShippingOrder>> GetShippingOrderByDailyOrderId(Guid dailyOrderId);  // Lấy ra các shippingOrder theo dailyOrderId 
}
