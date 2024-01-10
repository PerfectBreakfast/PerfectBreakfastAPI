using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Repositories;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RoleRepository(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }
    public async Task<List<IdentityRole<Guid>>> GetAllAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return roles;
    }

    public async Task<bool> AddAsync(IdentityRole<Guid> role)
    {
        var u = await _roleManager.CreateAsync(role);
        return u.Succeeded;
    }

    public async Task<bool> Update(IdentityRole<Guid> role)
    {
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> Delete(IdentityRole<Guid> role)
    {
        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<IdentityRole<Guid>> GetByIdAsync(Guid id, params Expression<Func<IdentityRole<Guid>, object>>[] includeProperties)
    {
        var result = await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
        // todo should throw exception when not found
        if (result == null)
            throw new NotFoundIdException($"Not Found by ID: [{id}] of [{typeof(IdentityRole)}]");
        return result;
    }

    public IQueryable<IdentityRole<Guid>> FindAll(params Expression<Func<IdentityRole<Guid>, object>>[]? includeProperties)
    {
        IQueryable<IdentityRole<Guid>> items = _roleManager.Roles.AsNoTracking();
        if(includeProperties != null)
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        return items;
    }

    public IQueryable<IdentityRole<Guid>> FindAll(Expression<Func<IdentityRole<Guid>, bool>>? predicate = null, params Expression<Func<IdentityRole<Guid>, object>>[]? includeProperties)
    {
        IQueryable<IdentityRole<Guid>> items = _roleManager.Roles.AsNoTracking();
        if(includeProperties != null)
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        return items.Where(predicate);
    }

    public async Task<IdentityRole<Guid>?> FindSingleAsync(Expression<Func<IdentityRole<Guid>, bool>>? predicate, params Expression<Func<IdentityRole<Guid>, object>>[]? includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(predicate);
    }
}
