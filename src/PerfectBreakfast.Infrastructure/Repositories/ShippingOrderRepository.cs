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
                .ThenInclude(x => x.MealSubscription)
                    .ThenInclude(x => x.Company)
            .ToListAsync();
        return shippingOrders;
    }

    public async Task<bool> ExistsWithDailyOrderAndShipper(Guid dailyOrderId, Guid shipperId)
    {
        return await _dbSet
            .AnyAsync(so => so.DailyOrderId == dailyOrderId && so.ShipperId == shipperId);
    }

    public async Task<bool> ExistsWithDailyOrderAndShippers(Guid dailyOrderId, List<Guid?> shipperId)
    {
        return await _dbSet
            .AnyAsync(so => so.DailyOrderId == dailyOrderId && shipperId.Contains(so.ShipperId.Value));
    }

    public async Task<List<ShippingOrder>> GetAllWithDailyOrdersAsync()
    {
        var shippingOrders = await _dbSet
            .Include(so => so.DailyOrder) 
            .ToListAsync();

        return shippingOrders;
    }
}
