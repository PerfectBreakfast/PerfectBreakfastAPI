using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IShippingOrderRepository : IGenericRepository<ShippingOrder>
{
    public Task<List<ShippingOrder>> GetShippingOrderByShipperId(Guid shipperId);
    public Task<bool> ExistsWithDailyOrderAndShipper(Guid dailyOrderId, Guid shipperId);
    public Task<bool> ExistsWithDailyOrderAndShippers(Guid dailyOrderId, List<Guid?> shipperId);

}
