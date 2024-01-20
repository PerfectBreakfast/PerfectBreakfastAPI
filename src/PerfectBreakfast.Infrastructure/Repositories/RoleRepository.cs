using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<Role> _roleManager;

    public RoleRepository(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }
    public async Task<List<Role>> GetAllAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return roles;
    }

    public async Task<bool> AddAsync(Role role)
    {
        var u = await _roleManager.CreateAsync(role);
        return u.Succeeded;
    }

    public async Task<bool> Update(Role role)
    {
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> Delete(Role role)
    {
        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<Role> GetByIdAsync(Guid id, params Expression<Func<Role, object>>[] includeProperties)
    {
        var result = await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
        // todo should throw exception when not found
        if (result == null)
            throw new NotFoundIdException($"Not Found by ID: [{id}] of [{typeof(Role)}]");
        return result;
    }

    public IQueryable<Role> FindAll(params Expression<Func<Role, object>>[]? includeProperties)
    {
        IQueryable<Role> items = _roleManager.Roles.AsNoTracking();
        if(includeProperties != null)
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        return items;
    }

    public IQueryable<Role> FindAll(Expression<Func<Role, bool>>? predicate = null, params Expression<Func<Role, object>>[]? includeProperties)
    {
        IQueryable<Role> items = _roleManager.Roles.AsNoTracking();
        if(includeProperties != null)
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        return items.Where(predicate);
    }

    public async Task<Role?> FindSingleAsync(Expression<Func<Role, bool>>? predicate, params Expression<Func<Role, object>>[]? includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(predicate);
    }
}
