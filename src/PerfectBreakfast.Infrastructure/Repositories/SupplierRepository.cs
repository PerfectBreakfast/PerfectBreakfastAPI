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
    public async Task<Supplier?> GetSupplierDetail(Guid id)
    {
        var supplier = await _dbSet.Where(x => x.Id == id)
            .Include(x => x.SupplyAssignments)
            .ThenInclude(x => x.Partner)
            .Include(x => x.SupplierCommissionRates)
            .ThenInclude(x => x.Food)
            .SingleOrDefaultAsync();
        return supplier;
    }

    public async Task<List<Supplier>?> GetSupplierByPartner(Guid id)
    {
        var suppliers = await _dbSet
            .Where(supplier => supplier.SupplyAssignments.Any(sa => sa.PartnerId == id))
            .Include(s => s.Users)
            .ToListAsync();
        return suppliers;
    }

    public async Task<Supplier?> GetSupplierById(Guid id, params Expression<Func<Supplier, object>>[] includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
    }

    public　async Task<List<Supplier>?> GetSupplierFoodAssignmentByPartner(Guid id)
    {
        var suppliers = await _dbSet
            .Where(supplier => supplier.SupplyAssignments.Any(sa => sa.PartnerId == id))
            .Include(s => s.SupplierCommissionRates)
            .ThenInclude(s => s.SupplierFoodAssignments)
            .ToListAsync();
        return suppliers;
    }

    public async Task<Supplier?> GetSupplierFoodAssignmentBySupplier(Guid id)
    {
        var suppliers = await _dbSet
            .Where(supplier => supplier.Id == id)
            .Include(s => s.SupplierCommissionRates)
            .ThenInclude(s => s.SupplierFoodAssignments)
            .SingleOrDefaultAsync();
        return suppliers;
    }
}