using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>,IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
    // to do

    public async Task<Pagination<User>> ToPagination(int pageIndex = 0, int pageSize = 10, Expression<Func<User, bool>>? predicate = null)
    {
        var itemCount = await _dbSet.CountAsync();
        IQueryable<User> itemsQuery = _dbSet.OrderByDescending(x => x.CreationDate);
        if (predicate != null)
        {
            itemsQuery = itemsQuery.Where(predicate);
        }
        var items = await itemsQuery
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        var result = new Pagination<User>()
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItemsCount = itemCount,
            Items = items,
        };
        return result;
    }

    /*public async Task<SignInResult> CheckPasswordSignin(User user, string password, bool lockoutOnFailure)
    {
        var isSuccess = await _signInManager
            .CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        return isSuccess;
    }*/

    public async Task<int> CalculateCompanyCode(Guid companyId)
    {
        var companyCode = await _dbSet.AsNoTracking()
            .Where(u => u.CompanyId == companyId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return companyCode + 1;
    }

    public async Task<int> CalculateDeliveryUnitCode(Guid deliveryUnitId)
    {
        var deliveryUnitCode = await _dbSet.AsNoTracking()
            .Where(u => u.DeliveryUnitId == deliveryUnitId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return deliveryUnitCode + 1;
    }

    public async Task<int> CalculateManagementUnitCode(Guid managementUnitId)
    {
        var managementUnitCode = await _dbSet.AsNoTracking()
            .Where(u => u.ManagementUnitId == managementUnitId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return managementUnitCode + 1;
    }

    public async Task<int> CalculateSupplierCode(Guid supplierId)
    {
        var supplierCode = await _dbSet.AsNoTracking()
            .Where(u => u.SupplierId == supplierId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return supplierCode + 1;
    }

    public async Task<List<User>?> GetUserByManagementUnitId(Guid managementUnitId)
    {
        /*var users = await _userManager.Users
            .Where(u => u.ManagementUnitId == managementUnitId)
            .ToListAsync();
        if (users != null && users.Any())
        {
            var usersWithRole = new List<User>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "MANAGEMENT UNIT ADMIN"))
                {
                    usersWithRole.Add(user);
                }
            }

            return usersWithRole;
        }*/
        return null;
    }
}