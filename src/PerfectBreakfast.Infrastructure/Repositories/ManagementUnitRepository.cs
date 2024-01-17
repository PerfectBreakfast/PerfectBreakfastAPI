using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class ManagementUnitRepository : GenericRepository<ManagementUnit>,IManagementUnitRepository
{
    public ManagementUnitRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) 
        : base(context, timeService, claimsService)
    {
    }
    // to do
    public async Task<List<ManagementUnit>> GetManagementUnits(DateTime dateTime)
    {
        var dateToCompare = dateTime.Date;
        return await _dbSet.
            Include(mu => mu.Companies)
            .ThenInclude(c => c.DailyOrders.Where(x => x.CreationDate.Date == dateToCompare))
                .ThenInclude(d => d.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Combo)
                            .ThenInclude(c => c.ComboFoods)
                                .ThenInclude(cf => cf.Food)
            .ToListAsync();

    }
}