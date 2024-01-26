using Microsoft.AspNetCore.Identity;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
   
    Task<Pagination<User>> ToPagination(int pageNumber = 0, int pageSize = 10, Expression<Func<User, bool>>? predicate = null,params IncludeInfo<User>[] includeProperties);
    public Task<int> CalculateCompanyCode(Guid companyId);
    public Task<int> CalculateDeliveryUnitCode(Guid deliveryUnitId);
    public Task<int> CalculateManagementUnitCode(Guid managementUnitId);
    public Task<int> CalculateSupplierCode(Guid supplierId);
    Task<List<User>?> GetUserByManagementUnitId(Guid managementUnitId);
    Task<User> GetUserByIdAsync(Guid id,params IncludeInfo<User>[]? includeProperties);
}