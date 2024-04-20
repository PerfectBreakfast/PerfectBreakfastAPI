using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class PackageRepositoryTests : SetupTest
{
    private readonly IPackageRepository _packageRepository;

    public PackageRepositoryTests()
    {
        _packageRepository = new PackageRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetByPartner_ReturnsPackagesWithDetails()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = new Partner
        {
            Id = partnerId,
            Name = "",
            Address = "",
            PhoneNumber = "",
            CommissionRate = 0
        };
        var meal = new Meal { Id = Guid.NewGuid(), MealType = "Healthy Meal" };
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Healthy Eats",
            PhoneNumber = "",
            Email = "",
            Address = ""
        };
        var mealSubscription = new MealSubscription { Id = Guid.NewGuid(), Meal = meal, Company = company };
        var dailyOrder = new DailyOrder { Id = Guid.NewGuid(), MealSubscription = mealSubscription };
        var food = new Food { Id = Guid.NewGuid(), Name = "Salad" };
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Local Farms",
            Address = "",
            PhoneNumber = ""
        };
        var supplierCommissionRate = new SupplierCommissionRate { Id = Guid.NewGuid(), Supplier = supplier, CommissionRate = 10.0m };
        var supplierFoodAssignment = new SupplierFoodAssignment { Id = Guid.NewGuid(), Food = food, SupplierCommissionRate = supplierCommissionRate };
        
        var package = new Package
        {
            Id = Guid.NewGuid(),
            Partner = partner,
            DailyOrder = dailyOrder,
            SupplierFoodAssignments = new List<SupplierFoodAssignment> { supplierFoodAssignment }
        };

        await _dbContext.Packages.AddAsync(package);
        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _packageRepository.GetByPartner(partnerId);

        // Assert
        results.Should().ContainSingle();
        var retrievedPackage = results.First();
        retrievedPackage.Partner.Should().Be(partner);
        retrievedPackage.DailyOrder.Should().Be(dailyOrder);
        retrievedPackage.DailyOrder.MealSubscription.Meal.Should().Be(meal);
        retrievedPackage.DailyOrder.MealSubscription.Company.Should().Be(company);
        retrievedPackage.SupplierFoodAssignments.First().Food.Should().Be(food);
        retrievedPackage.SupplierFoodAssignments.First().SupplierCommissionRate.Supplier.Should().Be(supplier);
    }
}