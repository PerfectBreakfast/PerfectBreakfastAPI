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
    
    public async Task<List<ShippingOrder>> GetShippingOrderTodayByShipperIdAndDate(Guid shipperId, DateOnly date)
    {
        var shippingOrders = await _dbSet.Where(x => x.ShipperId == shipperId && x.DailyOrder.BookingDate == date)
            .Include(x => x.DailyOrder)
                .ThenInclude(x => x.MealSubscription)
                    .ThenInclude(x => x.Company)
                        .ThenInclude(c => c.Partner)
            .Include(x => x.DailyOrder) // Repeating this Include to start a new chain
                    .ThenInclude(x => x.MealSubscription)
                        .ThenInclude(x => x.Meal)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();
        return shippingOrders;
    }

    public async Task<ShippingOrder?> GetShippingOrderByDailyOrderId(Guid shipperId, Guid dailyOrderId)
    {
        return await _dbSet.AsNoTracking()
            .SingleOrDefaultAsync(x => x.ShipperId == shipperId && x.DailyOrderId == dailyOrderId);
    }

    public async Task<bool> ExistsWithDailyOrderAndShippers(Guid dailyOrderId, List<Guid?> shipperId)
    {
        return await _dbSet.AsNoTracking()
            .AnyAsync(so => so.DailyOrderId == dailyOrderId && shipperId.Contains(so.ShipperId));
    }

    // lấy các staff đã phân công cho cái đơn (dailyOrder)
    public async Task<List<User?>> GetStaffByDailyOrder(Guid dailyOrderId)
    {
        return await _dbSet.AsNoTracking()
            .Where(so => so.DailyOrderId == dailyOrderId)
            .Include(x => x.Shipper)
            .Select(x => x.Shipper)
            .ToListAsync();
    }

    public async Task<List<ShippingOrder>> GetShippingOrderByDailyOrderId(Guid dailyOrderId)
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.DailyOrderId == dailyOrderId)
            .ToListAsync();
    }
    
}
