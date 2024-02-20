using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IShippingOrderRepository : IGenericRepository<ShippingOrder>
{
    public Task<List<ShippingOrder>> GetShippingOrderByShipperId(Guid shipperId);
}
