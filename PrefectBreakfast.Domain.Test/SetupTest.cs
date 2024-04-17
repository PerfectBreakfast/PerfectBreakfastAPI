using AutoFixture;
using Moq;
using PerfectBreakfast.Application.Interfaces;

namespace PrefectBreakfast.Domain.Test;

public class SetupTest : IDisposable
{
    protected readonly Fixture _fixture;
    protected readonly Mock<IUnitOfWork> __unitOfWorkMock;
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
    
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}