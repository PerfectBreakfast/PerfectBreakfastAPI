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

    public async Task<Partner?> GetPartnerDetail(Guid id)
    {
        return await _dbSet.Where(x => x.Id == id)
            .Include(x => x.Companies.Where(c => !c.IsDeleted))
            .Include(x => x.SupplyAssignments.Where(s => !s.Supplier.IsDeleted))
                .ThenInclude(x => x.Supplier)
            .AsNoTracking()
            .AsSplitQuery()
            .SingleOrDefaultAsync();
    }

    public async Task<Partner?> GetPartnerById(Guid id, params Expression<Func<Partner, object>>[] includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<List<Partner>> GetPartnersByToday(DateTime dateTime)
    {
        return await _dbSet.Include(mu => mu.Companies)
            .ToListAsync();
    }
}