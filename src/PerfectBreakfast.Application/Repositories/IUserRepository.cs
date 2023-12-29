using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IUserRepository //: IGenericRepository<User>
{
    public Task<int> CalculateCompanyCode(Guid companyId);
    public Task<int> CalculateDeliveryUnitCode(Guid deliveryUnitId);
    public Task<int> CalculateManagementUnitCode(Guid managementUnitId);
    public Task<int> CalculateSupplierCode(Guid supplierId);
}