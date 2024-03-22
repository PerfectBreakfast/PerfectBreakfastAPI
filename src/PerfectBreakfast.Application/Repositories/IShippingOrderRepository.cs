using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IShippingOrderRepository : IGenericRepository<ShippingOrder>
{
    public Task<List<ShippingOrder>> GetShippingOrderByShipperId(Guid shipperId);
    public Task<ShippingOrder?> GetShippingOrderByShipperIdAndDailyOrderId(Guid shipperId, Guid dailyOrderId);
    public Task<bool> ExistsWithDailyOrderAndShipper(Guid dailyOrderId, Guid shipperId);
    public Task<bool> ExistsWithDailyOrderAndShippers(Guid dailyOrderId, List<Guid?> shipperId);
    public Task<List<ShippingOrder>> GetAllWithDailyOrdersAsync();
    public Task<List<ShippingOrder>> GetShippingOrderByDailyOrder(Guid dailyOrderId);
    public Task<List<ShippingOrder>> GetShippingOrderByDailyOrderV2(Guid dailyOrderId,int pageNumber = 1, params IncludeInfo<ShippingOrder>[] includeProperties);
}
