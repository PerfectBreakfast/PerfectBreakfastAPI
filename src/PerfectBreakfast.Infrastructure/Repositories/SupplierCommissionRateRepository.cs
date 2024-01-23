using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class SupplierCommissionRateRepository : GenericRepository<SupplierCommissionRate>, ISupplierCommissionRateRepository
{
    public SupplierCommissionRateRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
    {
    }

    public async Task<bool> AnyAsync(Expression<Func<SupplierCommissionRate, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<List<SupplierCommissionRate>?> GetBySupplier(Guid id)
    {
        return await _dbSet.Where(s => s.SupplierId == id).ToListAsync();
    }
}