using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;


namespace PerfectBreakfast.Application.Repositories;
public interface IRoleRepository
{
    public Task<List<Role>> GetAllAsync();
    public Task<bool> AddAsync(Role role);
    public Task<bool> Update(Role role);
    public Task<bool> Delete(Role role);
    public Task<Role> GetByIdAsync(Guid id,params Expression<Func<Role, object>>[] includeProperties);
    public IQueryable<Role> FindAll(params Expression<Func<Role, object>>[]? includeProperties);
    public IQueryable<Role> FindAll(Expression<Func<Role, bool>>? predicate = null, params Expression<Func<Role, object>>[]? includeProperties);
    Task<Role?> FindSingleAsync(Expression<Func<Role, bool>>? predicate, params Expression<Func<Role, object>>[]? includeProperties);
}
