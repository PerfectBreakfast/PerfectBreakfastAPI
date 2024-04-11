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
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Company> GetCompanyDetailMealById(Guid companyId)
    {
        return await _dbSet.Include(c => c.Partner)
            .Include(c => c.Workers)
            .Include(c => c.Delivery)
            .Include(c => c.MealSubscriptions)
                .ThenInclude(ms => ms.Meal)
            .FirstOrDefaultAsync(c => c.Id == companyId);
    }

    public async Task<List<Company>> SearchCompany(string searchTerm, DateTime now)
    {
        var lowerCaseSearchTerm = searchTerm.ToLower();
        var twentyFourHoursAgo = now.AddHours(-24);
        return await _dbSet.Where(c => c.Name.ToLower().Contains(lowerCaseSearchTerm) && c.CreationDate <= twentyFourHoursAgo)
            .Take(2)
            .AsNoTracking()
            .ToListAsync();
    }
}