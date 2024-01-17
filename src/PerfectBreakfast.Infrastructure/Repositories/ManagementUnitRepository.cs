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
    public async Task<ManagementUnit?> GetManagementUintDetail(Guid id)
    {
        var managementUnit = await _dbSet.Where(x => x.Id == id)
            .Include(x => x.SupplyAssignments)
            .ThenInclude(x=> x.Supplier).SingleOrDefaultAsync();
        return managementUnit;


    }
}