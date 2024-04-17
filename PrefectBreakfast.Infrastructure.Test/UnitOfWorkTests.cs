using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test;

public class UnitOfWorkTests : SetupTest
{
    private readonly IUnitOfWork _unitOfWork;
    public ICategoryRepository CategoryRepository { get; }


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
    public async Task SaveChangeAsync_CallsSaveChangesAsyncOnDbContext()
    {

        // Adding a test entity, e.g., a Category if it's a part of your model
        _dbContext.Categories.Add(new Category { Name = "Test Category" });

        // Act
        var result = await _unitOfWork.SaveChangeAsync();

        // Assert
        Assert.Equal(1, result);  // Expecting one change to be saved (the addition of the Category)

        // Additional assertions can be made to check the actual data in the database if needed
        var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Name == "Test Category");
        Assert.True(categoryExists);
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
}