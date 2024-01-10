using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;


namespace PerfectBreakfast.Application.Repositories;
public interface IRoleRepository
{
    public Task<List<IdentityRole<Guid>>> GetAllAsync();
    public Task<bool> AddAsync(IdentityRole<Guid> role);
    public Task<bool> Update(IdentityRole<Guid> role);
    public Task<bool> Delete(IdentityRole<Guid> role);
    public Task<IdentityRole<Guid>> GetByIdAsync(Guid id,params Expression<Func<IdentityRole<Guid>, object>>[] includeProperties);
    public IQueryable<IdentityRole<Guid>> FindAll(params Expression<Func<IdentityRole<Guid>, object>>[]? includeProperties);
    public IQueryable<IdentityRole<Guid>> FindAll(Expression<Func<IdentityRole<Guid>, bool>>? predicate = null, params Expression<Func<IdentityRole<Guid>, object>>[]? includeProperties);
    Task<IdentityRole<Guid>?> FindSingleAsync(Expression<Func<IdentityRole<Guid>, bool>>? predicate, params Expression<Func<IdentityRole<Guid>, object>>[]? includeProperties);
}
