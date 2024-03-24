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

    public async Task<Pagination<User>> ToPagination(int pageIndex = 0, int pageSize = 10, 
        Expression<Func<User, bool>>? predicate = null,
        params IncludeInfo<User>[] includeProperties)
    {
        IQueryable<User> itemsQuery = _dbSet.OrderByDescending(x => x.CreationDate);
        if (predicate != null) 
        {
            itemsQuery = itemsQuery.Where(predicate); 
        }
        var itemCount = await itemsQuery.CountAsync();
        // Xử lý các thuộc tính include và thenInclude
        foreach (var includeProperty in includeProperties)
        {
            var queryWithInclude = itemsQuery.Include(includeProperty.NavigationProperty);
            foreach (var thenInclude in includeProperty.ThenIncludes)
            {
                queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
            }
            itemsQuery = queryWithInclude;
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

    public async Task<int> CalculateDeliveryCode(Guid deliveryUnitId)
    {
        var deliveryUnitCode = await _dbSet.AsNoTracking()
            .Where(u => u.DeliveryId == deliveryUnitId)
            .MaxAsync(u => (int?)u.Code) ?? 0;
        return deliveryUnitCode + 1;
    }

    public async Task<int> CalculatePartnerCode(Guid managementUnitId)
    {
        var managementUnitCode = await _dbSet.AsNoTracking()
            .Where(u => u.PartnerId == managementUnitId)
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

    public async Task<List<User>?> GetUserByPartnerId(Guid managementUnitId)
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

    public async Task<User> GetUserByIdAsync(Guid id, params IncludeInfo<User>[]? includeProperties)
    {
        var query  = _dbSet.AsNoTracking().Where(x => x.Id == id);
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                var queryWithInclude = query.Include(includeProperty.NavigationProperty);
                foreach (var thenInclude in includeProperty.ThenIncludes)
                {
                    queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
                }
                query = queryWithInclude;
            }
        }
        
        return await query.AsSplitQuery().SingleAsync();
    }

    public async Task<User?> GetInfoCurrentUserById(Guid id)
    {
        return await _dbSet.Where(x => x.Id == id)
            .Include(x => x.Company)
            .Include(x => x.Delivery)
            .Include(x => x.Partner)
            .Include(x => x.Supplier)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync();
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Email == email)
            .Include(x => x.UserRoles)!
            .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync();
    }


    public async Task<User> GetDeliveryStaffByDeliveryAdmin(Guid deliveryId)
    {
        var deliveryUnit = await _dbSet
            .Where(x => x.Id == deliveryId)
            .Include(x=>x.DeliveryId)
            .SingleOrDefaultAsync();
        return deliveryUnit;
    }

    //public async Task<User> CheckDeliveryStaff(Guid shipperId)
    //{
    //    //var id = await _dbSet
    //    //    .Where(x => x.Id == shipperId);
    //    //return id;
    //}
}