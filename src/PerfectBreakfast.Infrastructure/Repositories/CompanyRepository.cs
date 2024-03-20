using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
{
    public CompanyRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService)
        : base(context, timeService, claimsService)
    {
    }

    // to do
    public async Task<Company?> GetCompanyById(Guid id)
    {
        return await _dbSet.Include(c => c.MealSubscriptions)
                            .ThenInclude(c => c. DailyOrders)
                            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Company> GetCompanyDetailMealById(Guid companyId)
    {
        return await _dbSet.Include(c => c.Partner)
            .Include(c => c.Delivery)
            .Include(c => c.MealSubscriptions)
                .ThenInclude(ms => ms.Meal)
            .FirstOrDefaultAsync(c => c.Id == companyId);
    }

    public async Task<List<Company>> SearchCompany(string searchTerm)
    {
        var lowerCaseSearchTerm = searchTerm.ToLower();
        return await _dbSet.Where(c => c.Name.Contains(lowerCaseSearchTerm, StringComparison.CurrentCultureIgnoreCase)).Take(2).ToListAsync();
    }
}