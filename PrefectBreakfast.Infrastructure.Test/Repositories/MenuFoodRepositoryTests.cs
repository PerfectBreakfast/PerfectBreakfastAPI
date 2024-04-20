using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class MenuFoodRepositoryTests : SetupTest
{
    private readonly IMenuFoodRepository _menuFoodRepository;

    public MenuFoodRepositoryTests()
    {
        _menuFoodRepository = new MenuFoodRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task IsFoodExistInMenu_ReturnsTrueIfFoodExists()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var menuFood = new MenuFood { FoodId = foodId };
        _dbContext.MenuFoods.Add(menuFood);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _menuFoodRepository.IsFoodExistInMenu(foodId);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task IsFoodExistInMenu_ReturnsFalseIfFoodDoesNotExist()
    {
        // Arrange
        var nonExistentFoodId = Guid.NewGuid();

        // Act
        var exists = await _menuFoodRepository.IsFoodExistInMenu(nonExistentFoodId);

        // Assert
        exists.Should().BeFalse();
    }
    
    [Fact]
    public async Task IsComboExistInMenu_ReturnsTrueIfComboExists()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var menuFood = new MenuFood { ComboId = comboId };
        _dbContext.MenuFoods.Add(menuFood);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _menuFoodRepository.IsComboExistInMenu(comboId);

        // Assert
        exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task IsComboExistInMenu_ReturnsFalseIfComboDoesNotExist()
    {
        // Arrange
        var nonExistentComboId = Guid.NewGuid();

        // Act
        var exists = await _menuFoodRepository.IsComboExistInMenu(nonExistentComboId);

        // Assert
        exists.Should().BeFalse();
    }
}