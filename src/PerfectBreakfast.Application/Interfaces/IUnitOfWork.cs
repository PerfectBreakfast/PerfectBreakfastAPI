using Microsoft.EntityFrameworkCore.Storage;
using PerfectBreakfast.Application.Repositories;

namespace PerfectBreakfast.Application.Interfaces;

public interface IUnitOfWork
{
    public Task<int> SaveChangeAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    public IUserRepository UserRepository { get; }
    public ICompanyRepository CompanyRepository { get; }
    public ISupplierRepository SupplierRepository { get; }
    public IDeliveryUnitRepository DeliveryUnitRepository { get; }
    public IManagementUnitRepository ManagementUnitRepository { get; }
    public IRoleRepository RoleRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IFoodRepository FoodRepository { get; }
    
    public IMenuRepository MenuRepository { get; }
    public IPaymentMethodRepository PaymentMethodRepository { get; }

}
