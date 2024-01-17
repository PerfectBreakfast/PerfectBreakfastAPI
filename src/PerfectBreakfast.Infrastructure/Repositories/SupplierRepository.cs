using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class SupplierRepository : GenericRepository<Supplier>,ISupplierRepository
{
    public SupplierRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) 
        : base(context, timeService, claimsService)
    {
    }
    // to do
    public async Task<Supplier?> GetSupplierUintDetail(Guid id)
    {
        var supplier = await _dbSet.Where(x => x.Id == id)
            .Include(x => x.SupplyAssignments)
            .ThenInclude(x=> x.ManagementUnit).SingleOrDefaultAsync();
        return supplier;
    }
}