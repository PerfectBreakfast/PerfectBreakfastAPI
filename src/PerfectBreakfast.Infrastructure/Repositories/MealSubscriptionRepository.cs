using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class MealSubscriptionRepository : GenericRepository<MealSubscription>,IMealSubscriptionRepository
{
    public MealSubscriptionRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) 
        : base(context, timeService, claimsService)
    {
        
    }


    public async Task<MealSubscription?> FindByCompanyId(Guid companyId, Guid mealId)
    {
        return await _dbSet.Where(m => m.CompanyId == companyId && m.MealId == mealId)
                            .FirstOrDefaultAsync();
    }

    public async Task<List<MealSubscription>> GetByCompany(Guid companyId)
    {
        return await _dbSet.Where(m => m.CompanyId == companyId)
            .Include(m => m.DailyOrders)
            .Include(m => m.Meal)
            .ToListAsync();
    }
}