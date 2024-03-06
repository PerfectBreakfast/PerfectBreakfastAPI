using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class MealRepository : GenericRepository<Meal>,IMealRepository
{
    public MealRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) 
        : base(context, timeService, claimsService)
    {
        
    }
}