using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class FoodRepositoryTests : SetupTest
{
    private readonly IFoodRepository _foodRepository;

    public FoodRepositoryTests()
    {
        _foodRepository = new FoodRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetFoodForSupplier_ShouldReturnCorrectFoods()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var matchingFood = new Food { Id = Guid.NewGuid(), Name = "Apple", IsDeleted = false };
        var nonMatchingFood = new Food { Id = Guid.NewGuid(), Name = "Banana", IsDeleted = true };
        var nonMatchingFoodSupplier = new Food { Id = Guid.NewGuid(), Name = "Orange", IsDeleted = false };
        var supplierCommissionRate = new SupplierCommissionRate { SupplierId = supplierId, FoodId = nonMatchingFoodSupplier.Id };

        _dbContext.Foods.AddRange(matchingFood, nonMatchingFood, nonMatchingFoodSupplier);
        _dbContext.SupplierCommissionRates.Add(supplierCommissionRate);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _foodRepository.GetFoodForSupplier(supplierId, "Apple");

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(f => f.Id == matchingFood.Id);
    }
}