using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class PartnerRepository : GenericRepository<Partner>, IPartnerRepository
{
    public PartnerRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService)
        : base(context, timeService, claimsService)
    {
    }
    // to do
    public async Task<List<Partner>> GetPartners()
    {
        return await _dbSet.Include(mu => mu.Companies)
            .ThenInclude(c => c.DailyOrders)
            .ToListAsync();
    }

    public async Task<Partner?> GetPartnerDetail(Guid id)
    {
        var managementUnit = await _dbSet.Where(x => x.Id == id)
            .Include(x => x.SupplyAssignments)
            .ThenInclude(x => x.Supplier).SingleOrDefaultAsync();
        return managementUnit;

    }

    public async Task<Partner?> GetPartnerById(Guid id, params Expression<Func<Partner, object>>[] includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<List<Partner>> GetPartnersByToday(DateTime dateTime)
    {
        var dateToCompare = dateTime.Date;
        return await _dbSet.Include(mu => mu.Companies)
            .ThenInclude(c => c.DailyOrders.Where(x => x.CreationDate.Date == dateToCompare))
            .ToListAsync();
    }
}