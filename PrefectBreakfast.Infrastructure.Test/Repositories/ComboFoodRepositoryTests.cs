using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class ComboFoodRepositoryTests : SetupTest
{
    private readonly IComboFoodRepository _comboFoodRepository;

    public ComboFoodRepositoryTests()
    {
        _comboFoodRepository = new ComboFoodRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    
    [Fact]
    public async Task GetFoodByComboId_ReturnsFoodsAssociatedWithCombo()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var food1 = new Food { Id = Guid.NewGuid(), Name = "Pizza" };
        var food2 = new Food { Id = Guid.NewGuid(), Name = "Burger" };
        var comboFoods = new List<ComboFood>
        {
            new ComboFood { ComboId = comboId, Food = food1 },
            new ComboFood { ComboId = comboId, Food = food2 }
        };

        _dbContext.ComboFoods.AddRange(comboFoods);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _comboFoodRepository.GetFoodByComboId(comboId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Id == food1.Id && f.Name == "Pizza");
        result.Should().Contain(f => f.Id == food2.Id && f.Name == "Burger");
    }
    
    [Fact]
    public async Task IsFoodExistInCombo_ReturnsTrueIfFoodExistsInAnyCombo()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var comboFood = new ComboFood { FoodId = foodId, ComboId = Guid.NewGuid() };
        _dbContext.ComboFoods.Add(comboFood);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _comboFoodRepository.IsFoodExistInCombo(foodId);

        // Assert
        exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task IsFoodExistInCombo_ReturnsFalseIfFoodDoesNotExistInAnyCombo()
    {
        // Arrange
        var foodId = Guid.NewGuid(); // Ensuring no ComboFood has this foodId

        // Act
        var exists = await _comboFoodRepository.IsFoodExistInCombo(foodId);

        // Assert
        exists.Should().BeFalse();
    }
}