using Microsoft.AspNetCore.Identity;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
   
    public Task<Pagination<User>> ToPagination(int pageIndex = 0, int pageSize = 10, Expression<Func<User, bool>>? predicate = null);
    public Task<int> CalculateCompanyCode(Guid companyId);
    public Task<int> CalculateDeliveryUnitCode(Guid deliveryUnitId);
    public Task<int> CalculateManagementUnitCode(Guid managementUnitId);
    public Task<int> CalculateSupplierCode(Guid supplierId);
    Task<List<User>?> GetUserByManagementUnitId(Guid managementUnitId);
}