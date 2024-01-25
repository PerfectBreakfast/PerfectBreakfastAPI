using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
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
            .ThenInclude(x => x.ManagementUnit).SingleOrDefaultAsync();
        return supplier;
    }

    public async Task<List<Supplier>?> GetSupplierUnitByManagementUnit(Guid managementUnitId)
    {
        var suppliers = await _dbSet
            .Where(supplier => supplier.SupplyAssignments.Any(sa => sa.ManagementUnitId == managementUnitId))
            .ToListAsync();

        return suppliers;
    }

    public async Task<Supplier?> GetSupplierById(Guid id, params Expression<Func<Supplier, object>>[] includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
    }
}