using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test;

public class UnitOfWorkTests : SetupTest
{
    private readonly IUnitOfWork _unitOfWork;
    public UnitOfWorkTests()
    {
        _unitOfWork = new UnitOfWork(
            _dbContext,
            _currentTimeMock.Object,
            _claimServiceMock.Object,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _roleManagerMock.Object
        );
    }
    
    [Fact]
    public void Repositories_AreCorrectlyInitialized()
    {
        var unitOfWork = new UnitOfWork(
            _dbContext, _currentTimeMock.Object, _claimServiceMock.Object,
            _userManagerMock.Object, _signInManagerMock.Object, _roleManagerMock.Object
        );

        // Kiểm tra xem các repository có phải là instance đúng của class tương ứng không
        Assert.IsType<CategoryRepository>(unitOfWork.CategoryRepository);
        Assert.IsType<FoodRepository>(unitOfWork.FoodRepository);
        // Thêm các assert cho các repository khác
    }
    
    [Fact]
    public async Task SaveChangeAsync_CallsSaveChangesAsyncOnDbContext()
    {
        // Arrange
        _dbContext.Categories.Add(new Category { Name = "Test Category" });

        // Act
        var result = await _unitOfWork.SaveChangeAsync();

        // Assert
        Assert.Equal(1, result);  // Expecting one change to be saved
        var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Name == "Test Category");
        Assert.True(categoryExists);
    }

    [Fact]
    public void UserManager_ReturnsUserManager()
    {
        // Assert
        Assert.Equal(_userManagerMock.Object, _unitOfWork.UserManager);
    }

    [Fact]
    public void SignInManager_ReturnsSignInManager()
    {
        // Assert
        Assert.Equal(_signInManagerMock.Object, _unitOfWork.SignInManager);
    }

    [Fact]
    public void RoleManager_ReturnsRoleManager()
    {
        // Assert
        Assert.Equal(_roleManagerMock.Object, _unitOfWork.RoleManager);
    }

    // Tests for each repository property
    [Fact]
    public void UserRepository_IsOfTypeUserRepository()
    {
        Assert.IsType<UserRepository>(_unitOfWork.UserRepository);
    }

    [Fact]
    public void RoleRepository_IsOfTypeRoleRepository()
    {
        Assert.IsType<RoleRepository>(_unitOfWork.RoleRepository);
    }

    [Fact]
    public void CompanyRepository_IsOfTypeCompanyRepository()
    {
        Assert.IsType<CompanyRepository>(_unitOfWork.CompanyRepository);
    }

    [Fact]
    public void SupplierRepository_IsOfTypeSupplierRepository()
    {
        Assert.IsType<SupplierRepository>(_unitOfWork.SupplierRepository);
    }

    [Fact]
    public void DeliveryRepository_IsOfTypeDeliveryRepository()
    {
        Assert.IsType<DeliveryRepository>(_unitOfWork.DeliveryRepository);
    }

    [Fact]
    public void PartnerRepository_IsOfTypePartnerRepository()
    {
        Assert.IsType<PartnerRepository>(_unitOfWork.PartnerRepository);
    }

    [Fact]
    public void CategoryRepository_IsOfTypeCategoryRepository()
    {
        Assert.IsType<CategoryRepository>(_unitOfWork.CategoryRepository);
    }

    [Fact]
    public void FoodRepository_IsOfTypeFoodRepository()
    {
        Assert.IsType<FoodRepository>(_unitOfWork.FoodRepository);
    }
    
    [Fact]
    public void MenuRepository_IsOfTypeMenuRepository()
    {
        Assert.IsType<MenuRepository>(_unitOfWork.MenuRepository);
    }
    
    [Fact]
    public void PaymentMethodRepository_IsOfTypePaymentMethodRepository()
    {
        Assert.IsType<PaymentMethodRepository>(_unitOfWork.PaymentMethodRepository);
    }
    
    // [Fact]
    // public void ComboRepository_IsOfTypeComboRepository()
    // {
    //     Assert.IsType<ComboRepository>(_unitOfWork.ComboRepository);
    // }
    
    // [Fact]
    // public void DailyOrderRepository_IsOfTypeDailyOrderRepository()
    // {
    //     Assert.IsType<DailyOrderRepository>(_unitOfWork.DailyOrderRepository);
    // }
    //
    // [Fact]
    // public void OrderRepository_IsOfTypeOrderRepository()
    // {
    //     Assert.IsType<OrderRepository>(_unitOfWork.OrderRepository);
    // }
}