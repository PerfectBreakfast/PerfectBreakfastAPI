using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IShippingOrderRepository : IGenericRepository<ShippingOrder>
{
    public Task<List<ShippingOrder>> GetShippingOrderByShipperId(Guid shipperId);
    public Task<List<ShippingOrder>> GetShippingOrderTodayByShipperIdAndDate(Guid shipperId,DateOnly date);
    public Task<ShippingOrder?> GetShippingOrderByDailyOrderId(Guid shipperId, Guid dailyOrderId);
    public Task<bool> ExistsWithDailyOrderAndShipper(Guid dailyOrderId, Guid shipperId);
    public Task<bool> ExistsWithDailyOrderAndShippers(Guid dailyOrderId, List<Guid?> shipperId);
    public Task<List<ShippingOrder>> GetAllWithDailyOrdersAsync();
    public Task<List<ShippingOrder>> GetShippingOrderByDailyOrder(Guid dailyOrderId);
    public Task<List<ShippingOrder>> GetShippingOrderByDailyOrderId(Guid dailyOrderId);  // Lấy ra các shippingOrder theo dailyOrderId 
}
