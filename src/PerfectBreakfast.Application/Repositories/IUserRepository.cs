using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IUserRepository 
{
    public Task<List<User>> GetAllAsync();
    public Task<bool> AddToRole(User user, string role);
    public Task<bool> AddAsync(User user,string password);
    public Task<bool> Update(User user);
    public Task<User> GetByIdAsync(Guid id,params Expression<Func<User, object>>[] includeProperties);
    public Task<Pagination<User>> ToPagination(int pageIndex = 0, int pageSize = 10, Expression<Func<User, bool>>? predicate = null);
    public Task<SignInResult> CheckPasswordSignin(User user, string password, bool lockoutOnFailure);
    public Task<int> CalculateCompanyCode(Guid companyId);
    public Task<int> CalculateDeliveryUnitCode(Guid deliveryUnitId);
    public Task<int> CalculateManagementUnitCode(Guid managementUnitId);
    public Task<int> CalculateSupplierCode(Guid supplierId);
    public IQueryable<User> FindAll(params Expression<Func<User, object>>[]? includeProperties);
    public IQueryable<User> FindAll(Expression<Func<User, bool>>? predicate = null, params Expression<Func<User, object>>[]? includeProperties);
    Task<User?> FindSingleAsync(Expression<Func<User, bool>>? predicate, params Expression<Func<User, object>>[]? includeProperties);
}