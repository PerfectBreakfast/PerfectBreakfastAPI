using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;


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
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();
        return shippingOrders;
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

    public async Task<bool> ExistsWithDailyOrderAndShipper(Guid dailyOrderId, Guid shipperId)
    {
        return await _dbSet
            .AnyAsync(so => so.DailyOrderId == dailyOrderId && so.ShipperId == shipperId);
    }

    public async Task<bool> ExistsWithDailyOrderAndShippers(Guid dailyOrderId, List<Guid?> shipperId)
    {
        return await _dbSet
            .AnyAsync(so => so.DailyOrderId == dailyOrderId && shipperId.Contains(so.ShipperId));
    }

    public async Task<List<ShippingOrder>> GetAllWithDailyOrdersAsync()
    {
        var shippingOrders = await _dbSet
            .Include(so => so.DailyOrder) 
            .ToListAsync();

        return shippingOrders;
    }

    public async Task<List<ShippingOrder>> GetShippingOrderByDailyOrder(Guid dailyOrderId)
    {
        return await _dbSet.Where(s => s.DailyOrderId == dailyOrderId && s.Status == ShippingStatus.Pending)
            .Include(s => s.Shipper)
            .Include(s => s.DailyOrder)
                .ThenInclude(d => d.MealSubscription)
            .ToListAsync();
    }

    public async Task<List<ShippingOrder>> GetShippingOrderByDailyOrderV2(Guid dailyOrderId, int pageNumber = 1, params IncludeInfo<ShippingOrder>[] includeProperties)
    {
        var itemsQuery = _dbSet.Where(x => x.DailyOrder.Id == dailyOrderId);
        itemsQuery = itemsQuery.OrderByDescending(x => x.DailyOrder.BookingDate);
        itemsQuery = itemsQuery.Take(pageNumber);
        
        // Xử lý các thuộc tính include và thenInclude
        foreach (var includeProperty in includeProperties)
        {
            var queryWithInclude = itemsQuery.Include(includeProperty.NavigationProperty);
            foreach (var thenInclude in includeProperty.ThenIncludes)
            {
                queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
            }
            itemsQuery = queryWithInclude;
        }
        return await itemsQuery.AsNoTracking().ToListAsync();
    }
}
