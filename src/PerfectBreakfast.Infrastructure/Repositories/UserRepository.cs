using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private UserManager<User> _userManager;
    public UserRepository(UserManager<User> userManager )
    {
        _userManager = userManager;
    }
    // to do
    public async Task<int> CalculateCompanyCode(Guid companyId)
    {
        var companyCode = await _userManager.Users
            .Where(u => u.CompanyId == companyId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return companyCode + 1;
    }

    public async Task<int> CalculateDeliveryUnitCode(Guid deliveryUnitId)
    {
        var deliveryUnitCode = await _userManager.Users
            .Where(u => u.DeliveryUnitId == deliveryUnitId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return deliveryUnitCode + 1;
    }

    public async Task<int> CalculateManagementUnitCode(Guid managementUnitId)
    {
        var managementUnitCode = await _userManager.Users
            .Where(u => u.ManagementUnitId == managementUnitId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return managementUnitCode + 1;
    }

    public async Task<int> CalculateSupplierCode(Guid supplierId)
    {
        var supplierCode = await _userManager.Users
            .Where(u => u.SupplierId == supplierId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return supplierCode + 1;
    }
}