using Microsoft.AspNetCore.Identity;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
   
    Task<Pagination<User>> ToPagination(int pageNumber = 0, int pageSize = 10, Expression<Func<User, bool>>? predicate = null,params IncludeInfo<User>[] includeProperties);
    public Task<int> CalculateCompanyCode(Guid companyId);
    public Task<int> CalculateDeliveryCode(Guid deliveryId);
    public Task<int> CalculatePartnerCode(Guid partnerId);
    public Task<int> CalculateSupplierCode(Guid supplierId);
    Task<List<User>?> GetUserByPartnerId(Guid partnerId);
    Task<User> GetUserByIdAsync(Guid id,params IncludeInfo<User>[]? includeProperties);
}