using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;
using System.Text;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    // to do
    public async Task<List<User>> GetAllAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<bool> AddToRole(User user, string role)
    {
        var result = await _userManager.AddToRoleAsync(user, role);
        if (result.Succeeded) return result.Succeeded;
        var builder = new StringBuilder();
        foreach (var identityError in result.Errors)
        {
            builder.Append(identityError.Description);
            builder.Append(", ");
        }
        throw new IdentityException(builder.ToString());
    }

    public async Task<bool> AddAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded) return result.Succeeded;
        var builder = new StringBuilder();
        foreach (var identityError in result.Errors)
        {
            builder.Append(identityError.Description);
            builder.Append(". ");
        }
        throw new IdentityException(builder.ToString());
    }

    public async Task<bool> Update(User user)
    {
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded) return result.Succeeded;
        var builder = new StringBuilder();
        foreach (var identityError in result.Errors)
        {
            builder.Append(identityError.Description);
            builder.Append(", ");
        }
        throw new IdentityException(builder.ToString());
    }

    public async Task<User> GetByIdAsync(Guid id, params Expression<Func<User, object>>[] includeProperties)
    {
        var result = await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
        // todo should throw exception when not found
        if (result == null)
            throw new NotFoundIdException($"Not Found by ID: [{id}] of [{typeof(User)}]");
        return result;
    }

    public async Task<Pagination<User>> ToPagination(int pageIndex = 0, int pageSize = 10, Expression<Func<User, bool>>? predicate = null)
    {
        var itemCount = await _userManager.Users.CountAsync();
        IQueryable<User> itemsQuery = _userManager.Users.OrderByDescending(x => x.CreationDate);
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

    public async Task<SignInResult> CheckPasswordSignin(User user, string password, bool lockoutOnFailure)
    {
        var isSuccess = await _signInManager
            .CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        return isSuccess;
    }

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

    public IQueryable<User> FindAll(params Expression<Func<User, object>>[]? includeProperties)
    {
        IQueryable<User> items = _userManager.Users.AsNoTracking();
        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        return items;
    }

    public IQueryable<User> FindAll(Expression<Func<User, bool>>? predicate = null, params Expression<Func<User, object>>[]? includeProperties)
    {
        IQueryable<User> items = _userManager.Users.AsNoTracking();
        if (includeProperties != null)
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        return items.Where(predicate);
    }

    public async Task<User?> FindSingleAsync(Expression<Func<User, bool>>? predicate, params Expression<Func<User, object>>[]? includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(predicate);
    }

    public async Task<List<User>?> GetUserByManagementUnitId(Guid managementUnitId)
    {
        var users = await _userManager.Users
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
        }
        return null;
    }
}