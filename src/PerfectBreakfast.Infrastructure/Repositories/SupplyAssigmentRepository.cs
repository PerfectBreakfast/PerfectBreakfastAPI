﻿using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class SupplyAssigmentRepository : BaseRepository<SupplyAssignment>,ISupplyAssigmentRepository
{
    public SupplyAssigmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<SupplyAssignment>> GetByManagementUnitID(Guid managementUnitId)
    {
        return await _dbSet
            .Where(sa => sa.ManagementUnitId == managementUnitId)
            .ToListAsync();
    }
    public async Task<List<SupplyAssignment>> GetBySupplierID(Guid supplierId)
    {
        return await _dbSet
            .Where(sa => sa.SupplierId == supplierId)
            .ToListAsync();
    }
}