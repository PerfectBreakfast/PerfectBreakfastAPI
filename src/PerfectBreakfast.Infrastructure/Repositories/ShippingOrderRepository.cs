using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;


namespace PerfectBreakfast.Infrastructure.Repositories;

public class ShippingOrderRepository : GenericRepository<ShippingOrder>, IShippingOrderRepository
{
    public ShippingOrderRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
    {
    }

    public async Task<List<ShippingOrder>> GetShippingOrderByShipperId(Guid shipperId)
    {
        var shippingOrders = await _dbSet.Where(x => x.ShipperId == shipperId)
            .Include(x => x.DailyOrder)
            .ThenInclude(x => x.Company).ToListAsync();
        return shippingOrders;
    }
}
