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
}