using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class SupplierCommissionRateRepositoryTests : SetupTest
{
    private readonly ISupplierCommissionRateRepository _supplierCommissionRateRepository;

    public SupplierCommissionRateRepositoryTests()
    {
        _supplierCommissionRateRepository =
            new SupplierCommissionRateRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task AnyAsync_ReturnsTrue_WhenPredicateIsMet()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var foodId = Guid.NewGuid();
        var commissionRate = new SupplierCommissionRate { SupplierId = supplierId,FoodId = foodId, CommissionRate = 10.0M };
        _dbContext.SupplierCommissionRates.Add(commissionRate);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _supplierCommissionRateRepository.AnyAsync(s => s.SupplierId == supplierId && s.FoodId == foodId);

        // Assert
        exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task AnyAsync_ReturnsFalse_WhenPredicateIsNotMet()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var commissionRate = new SupplierCommissionRate { SupplierId = supplierId, CommissionRate = 10.0M };
        _dbContext.SupplierCommissionRates.Add(commissionRate);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _supplierCommissionRateRepository.AnyAsync(s => s.SupplierId == Guid.NewGuid());

        // Assert
        exists.Should().BeFalse();
    }
    
    [Fact]
    public async Task GetBySupplierId_ReturnsCommissionRatesWithFood()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var food = new Food { Name = "Apple", Id = Guid.NewGuid() };
        var commissionRate = new SupplierCommissionRate { SupplierId = supplierId, Food = food };
        _dbContext.SupplierCommissionRates.Add(commissionRate);
        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _supplierCommissionRateRepository.GetBySupplierId(supplierId);

        // Assert
        results.Should().ContainSingle().Which.Food.Should().NotBeNull();
        results.First().Food.Name.Should().Be("Apple");
    }
}