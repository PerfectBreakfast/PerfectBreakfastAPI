using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class SupplyAssigmentRepository : BaseRepository<SupplyAssignment>,ISupplyAssigmentRepository
{
    public SupplyAssigmentRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<bool> IsDuplicateAssignment(Guid partnerId, Guid supplierId)
    {
        return await _dbSet.AsNoTracking()
            .AnyAsync(sa => sa.PartnerId == partnerId && sa.SupplierId == supplierId);
    }
    
}