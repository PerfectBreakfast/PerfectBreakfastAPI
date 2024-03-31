using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class PackageRepository : GenericRepository<Package>, IPackageRepository
{
    public PackageRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
    {
    }

    public Task<List<Package>> GetByPartner(Guid? partnerId)
    {
        return _dbSet.Where(p => p.Partner.Id == partnerId)
            .Include(p => p.DailyOrder)
                .ThenInclude(d => d.MealSubscription)
                    .ThenInclude(d => d.Meal)
            .Include(p => p.DailyOrder)
                .ThenInclude(d => d.MealSubscription)
                    .ThenInclude(d => d.Company)
            .Include(p => p.SupplierFoodAssignments)
                .ThenInclude(s => s.Food)
            .Include(p => p.SupplierFoodAssignments)
                .ThenInclude(s => s.SupplierCommissionRate)
                    .ThenInclude(s => s.Supplier)
            .ToListAsync();
    }
}