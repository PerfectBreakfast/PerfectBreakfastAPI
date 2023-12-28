using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context,ICurrentTime currentTime, IClaimsService claimsService) 
        : base(context,currentTime,claimsService)
    {
        
    }
    // to do
    public async Task<int> CalculateCompanyCode(Guid companyId)
    {
        var companyCode = await _dbSet
            .Where(u => u.CompanyId == companyId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return companyCode + 1;
    }

    public async Task<int> CalculateDeliveryUnitCode(Guid deliveryUnitId)
    {
        var deliveryUnitCode = await _dbSet
            .Where(u => u.DeliveryUnitId == deliveryUnitId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return deliveryUnitCode + 1;
    }

    public async Task<int> CalculateManagementUnitCode(Guid managementUnitId)
    {
        var managementUnitCode = await _dbSet
            .Where(u => u.ManagementUnitId == managementUnitId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return managementUnitCode + 1;
    }

    public async Task<int> CalculateSupplierCode(Guid supplierId)
    {
        var supplierCode = await _dbSet
            .Where(u => u.SupplierId == supplierId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return supplierCode + 1;
    }
}