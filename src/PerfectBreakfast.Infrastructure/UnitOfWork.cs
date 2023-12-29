using Microsoft.EntityFrameworkCore.Storage;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Infrastructure.Repositories;

namespace PerfectBreakfast.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentTime _currentTime;
    private readonly IClaimsService _claimsService;


    public UnitOfWork(AppDbContext dbContext, ICurrentTime currentTime, IClaimsService claimsService)
    {
        _dbContext = dbContext;
        _currentTime = currentTime;
        _claimsService = claimsService;
    }
    public async Task<int> SaveChangeAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }

    public IUserRepository UserRepository => new UserRepository(_dbContext, _currentTime, _claimsService);
    public ICompanyRepository CompanyRepository => new CompanyRepository(_dbContext, _currentTime, _claimsService);
    public ISupplierRepository SupplierRepository => new SupplierRepository(_dbContext, _currentTime, _claimsService);
    public IDeliveryUnitRepository DeliveryUnitRepository => new DeliveryUnitRepository(_dbContext, _currentTime, _claimsService);
    public IManagementUnitRepository ManagementUnitRepository => new ManagementUnitRepository(_dbContext, _currentTime, _claimsService);
    public IRoleRepository RoleRepository => new RoleRepository(_dbContext, _currentTime, _claimsService);
    public ICategoryRepository CategoryRepository => new CategoryRepository(_dbContext, _currentTime, _claimsService);
    public IFoodRepository FoodRepository => new FoodRepository(_dbContext, _currentTime, _claimsService);

    public IMenuRepository MenuRepository => new MenuRepository(_dbContext, _currentTime, _claimsService);
    public IPaymentMethodRepository PaymentMethodRepository => new PaymentMethodRepository(_dbContext, _currentTime, _claimsService);
}
