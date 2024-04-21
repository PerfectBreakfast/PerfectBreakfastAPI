using AutoFixture;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure;
using PerffectBreakfast.Infrastructure.Mappers;

namespace PrefectBreakfast.Domain.Test;

public class SetupTest : IDisposable
{
    protected readonly Fixture _fixture;
    protected readonly Mock<IUnitOfWork> _unitOfWorkMock;
    protected readonly IMapper _mapper;  
    protected readonly Mock<IServiceProvider> _serviceProviderMock;

    
    // Add new mocks for Identity
    protected readonly Mock<UserManager<User>> _userManagerMock;
    protected readonly Mock<SignInManager<User>> _signInManagerMock;
    protected readonly Mock<RoleManager<Role>> _roleManagerMock;
    
    
    protected readonly Mock<ICategoryService> _categoryServiceMock;
    protected readonly Mock<IClaimsService> _claimServiceMock;
    protected readonly Mock<IComboService> _comboServiceMock;
    protected readonly Mock<ICompanyService> _companyServiceMock;
    protected readonly Mock<ICurrentTime> _currentTimeMock;
    protected readonly Mock<IDailyOrderService> _dailyOrderServiceMock;
    protected readonly Mock<IDashboardService> _dashboardServiceMock;
    protected readonly Mock<IDeliveryService> _deliveryServiceMock;
    protected readonly Mock<IExportExcelService> _exportExcelServiceMock;
    protected readonly Mock<IFoodService> _foodServiceMock;
    protected readonly Mock<IHangfireSettingService> _hangfireSettingServiceMock;
    protected readonly Mock<IImgurService> _imgurServiceMock;
    protected readonly Mock<IJwtService> _jwtServiceMock;
    protected readonly Mock<IMailService> _mailServiceMock;
    protected readonly Mock<IManagementService> _managementServiceMock;
    protected readonly Mock<IMealService> _mealServiceMock;
    protected readonly Mock<IMenuService> _menuServiceMock;
    protected readonly Mock<IOrderService> _orderServiceMock;
    protected readonly Mock<IPackageService> _packageServiceMock;
    protected readonly Mock<IPartnerService> _partnerServiceMock;
    protected readonly Mock<IPaymentMethodService> _paymentServiceMock;
    protected readonly Mock<IPayOsService> _payOsServiceMock;
    protected readonly Mock<IRoleService> _roleServiceMock;
    protected readonly Mock<IShippingOrderService> _shippingOrderServiceMock;
    protected readonly Mock<ISupplierCommissionRateService> _supplierCommissionRateServiceMock;
    protected readonly Mock<ISupplierFoodAssignmentService> _supplierFoodAssignmentServiceMock;
    protected readonly Mock<ISupplierService> _supplierServiceMock;
    protected readonly Mock<ISupplyAssigmentService> _supplyAssigmentServiceMock;
    protected readonly Mock<IUserService> _userServiceMock;
    
    protected readonly AppDbContext _dbContext;

    public SetupTest()
    {
        _fixture = new Fixture();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        // Setup actual Mapster mapper
        var config = new TypeAdapterConfig();
        new MapsterConfig().Register(config);
        
        // Mock the IServiceProvider
        _serviceProviderMock = new Mock<IServiceProvider>();
        _mapper = new ServiceMapper(_serviceProviderMock.Object,config); // Assuming ServiceMapper is your IMapper implementation

        
        // Mocking UserManager
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        // Mocking RoleManager
        var roleStoreMock = new Mock<IRoleStore<Role>>();
        _roleManagerMock = new Mock<RoleManager<Role>>(roleStoreMock.Object, null, null, null, null);

        // Mocking SignInManager
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);

        
        // Services
    _categoryServiceMock = new Mock<ICategoryService>();
    _claimServiceMock = new Mock<IClaimsService>();
    _comboServiceMock = new Mock<IComboService>();
    _companyServiceMock = new Mock<ICompanyService>();
    _currentTimeMock = new Mock<ICurrentTime>();
    _dailyOrderServiceMock = new Mock<IDailyOrderService>();
    _dashboardServiceMock = new Mock<IDashboardService>();
    _deliveryServiceMock = new Mock<IDeliveryService>();
    _exportExcelServiceMock = new Mock<IExportExcelService>();
    _foodServiceMock = new Mock<IFoodService>();
    _hangfireSettingServiceMock = new Mock<IHangfireSettingService>();
    _imgurServiceMock = new Mock<IImgurService>();
    _jwtServiceMock = new Mock<IJwtService>();
    _mailServiceMock = new Mock<IMailService>();
    _managementServiceMock = new Mock<IManagementService>();
    _mealServiceMock = new Mock<IMealService>();
    _menuServiceMock = new Mock<IMenuService>();
    _orderServiceMock = new Mock<IOrderService>();
    _packageServiceMock = new Mock<IPackageService>();
    _partnerServiceMock = new Mock<IPartnerService>();
    _paymentServiceMock = new Mock<IPaymentMethodService>();
    _payOsServiceMock = new Mock<IPayOsService>();
    _roleServiceMock = new Mock<IRoleService>();
    _shippingOrderServiceMock = new Mock<IShippingOrderService>();
    _supplierCommissionRateServiceMock = new Mock<ISupplierCommissionRateService>();
    _supplierFoodAssignmentServiceMock = new Mock<ISupplierFoodAssignmentService>();
    _supplierServiceMock = new Mock<ISupplierService>();
    _supplyAssigmentServiceMock = new Mock<ISupplyAssigmentService>();
    _userServiceMock = new Mock<IUserService>();
    
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    _dbContext = new AppDbContext(options);

    _currentTimeMock.Setup(x => x.GetCurrentTime()).Returns(DateTime.UtcNow);
    _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.Empty);
    }
    
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}