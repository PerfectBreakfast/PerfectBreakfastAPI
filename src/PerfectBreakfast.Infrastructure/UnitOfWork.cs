using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;

namespace PerfectBreakfast.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentTime _currentTime;
    private readonly IClaimsService _claimsService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;
    
    public UnitOfWork(AppDbContext dbContext, ICurrentTime currentTime
        , IClaimsService claimsService, UserManager<User> userManager
        , SignInManager<User> signInManager, RoleManager<Role> roleManager)
    {
        _dbContext = dbContext;
        _currentTime = currentTime;
        _claimsService = claimsService;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;

    }
    public async Task<int> SaveChangeAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public IDbTransaction BeginTransaction()
    {
        return _dbContext.Database.BeginTransaction().GetDbTransaction();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
    public UserManager<User> UserManager => _userManager;
    public SignInManager<User> SignInManager => _signInManager;
    public RoleManager<Role> RoleManager => _roleManager;

    public IUserRepository UserRepository => new UserRepository(_dbContext);
    public IRoleRepository RoleRepository => new RoleRepository(_dbContext);
    public ICompanyRepository CompanyRepository => new CompanyRepository(_dbContext, _currentTime, _claimsService);
    public ISupplierRepository SupplierRepository => new SupplierRepository(_dbContext, _currentTime, _claimsService);
    public IDeliveryRepository DeliveryRepository => new DeliveryRepository(_dbContext, _currentTime, _claimsService);
    public IPartnerRepository PartnerRepository => new PartnerRepository(_dbContext, _currentTime, _claimsService);
    public ICategoryRepository CategoryRepository => new CategoryRepository(_dbContext, _currentTime, _claimsService);
    public IFoodRepository FoodRepository => new FoodRepository(_dbContext, _currentTime, _claimsService);
    public IMenuRepository MenuRepository => new MenuRepository(_dbContext, _currentTime, _claimsService);
    public IPaymentMethodRepository PaymentMethodRepository => new PaymentMethodRepository(_dbContext, _currentTime, _claimsService);
    public IComboRepository ComboRepository => new ComboRepository(_dbContext, _currentTime, _claimsService);
    public IDailyOrderRepository DailyOrderRepository => new DailyOrderRepository(_dbContext, _currentTime, _claimsService);
    public IOrderRepository OrderRepository => new OrderRepository(_dbContext, _currentTime, _claimsService);
    public ISupplierCommissionRateRepository SupplierCommissionRateRepository =>
        new SupplierCommissionRateRepository(_dbContext, _currentTime, _claimsService);
    public ISupplyAssigmentRepository SupplyAssigmentRepository => new SupplyAssigmentRepository(_dbContext);
    public ISupplierFoodAssignmentRepository SupplierFoodAssignmentRepository => new SupplierFoodAssignmentRepository(_dbContext, _currentTime, _claimsService);
    public IShippingOrderRepository ShippingOrderRepository => new ShippingOrderRepository(_dbContext, _currentTime, _claimsService);   
    public IMealRepository MealRepository => new MealRepository(_dbContext, _currentTime, _claimsService);   
    public IMealSubscriptionRepository MealSubscriptionRepository => new MealSubscriptionRepository(_dbContext, _currentTime, _claimsService);
    public IMenuFoodRepository MenuFoodRepository => new MenuFoodRepository(_dbContext, _currentTime, _claimsService);
    public IComboFoodRepository ComboFoodRepository => new ComboFoodRepository(_dbContext, _currentTime, _claimsService);
    public ISettingRepository SettingRepository => new SettingRepository(_dbContext, _currentTime, _claimsService);
    public IActionHistoryRepository ActionHistoryRepository => new ActionHistoryRepository(_dbContext, _currentTime, _claimsService);
}
