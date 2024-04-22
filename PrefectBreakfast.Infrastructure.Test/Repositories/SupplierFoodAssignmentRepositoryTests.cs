using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class SupplierFoodAssignmentRepositoryTests : SetupTest
{
    private readonly ISupplierFoodAssignmentRepository _supplierFoodAssignmentRepository;

    public SupplierFoodAssignmentRepositoryTests()
    {
        _supplierFoodAssignmentRepository =
            new SupplierFoodAssignmentRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetForSupplierByPackage_ReturnsFilteredAssignments()
    {
        var supplierId = Guid.NewGuid();
        var packageId = Guid.NewGuid();
        var assignment = new SupplierFoodAssignment
        {
            SupplierCommissionRate = new SupplierCommissionRate { SupplierId = supplierId },
            Package = new Package { Id = packageId },
            Status = SupplierFoodAssignmentStatus.Confirmed
        };

        _dbContext.SupplierFoodAssignments.Add(assignment);
        await _dbContext.SaveChangesAsync();

        var result = await _supplierFoodAssignmentRepository.GetForSupplierByPackage(new List<Guid> { packageId }, supplierId);

        result.Should().ContainSingle();
        result.First().SupplierCommissionRate.SupplierId.Should().Be(supplierId);
    }
    
    [Fact]
    public async Task GetForPartnerByPackage_IncludesRequiredEntities()
    {
        var partnerId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var packageId = Guid.NewGuid();
        var assignment = new SupplierFoodAssignment
        {
            Package = new Package
            {
                Id = packageId,
                PartnerId = partnerId,
                DailyOrder = new DailyOrder { MealSubscription = new MealSubscription { Meal = new Meal
                {
                    MealType = string.Empty
                }, Company = new Company
                    {
                        Name = string.Empty,
                        PhoneNumber = string.Empty,
                        Email = string.Empty,
                        Address = string.Empty
                    }
                } }
            },
            SupplierCommissionRate = new SupplierCommissionRate { SupplierId = supplierId },
        };

        _dbContext.SupplierFoodAssignments.Add(assignment);
        await _dbContext.SaveChangesAsync();

        var result = await _supplierFoodAssignmentRepository.GetForPartnerByPackage(new List<Guid> { packageId }, partnerId, supplierId);

        result.Should().ContainSingle();
        result.First().Package.DailyOrder.MealSubscription.Meal.Should().NotBeNull();
        result.First().Package.DailyOrder.MealSubscription.Company.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetByPackage_ReturnsCorrectAssignments()
    {
        var packageId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var assignment = new SupplierFoodAssignment
        {
            PackageId = packageId,
            SupplierCommissionRate = new SupplierCommissionRate { SupplierId = supplierId }
        };

        _dbContext.SupplierFoodAssignments.Add(assignment);
        await _dbContext.SaveChangesAsync();

        var result = await _supplierFoodAssignmentRepository.GetByPackages(packageId, supplierId);

        result.Should().ContainSingle();
        result.First().PackageId.Should().Be(packageId);
    }
    
    [Fact]
    public async Task GetByPackage_ReturnsOnlyAssignmentsForGivenPackageId()
    {
        // Arrange
        var targetPackageId = Guid.NewGuid();
        var otherPackageId = Guid.NewGuid();
        var assignments = new List<SupplierFoodAssignment>
        {
            new SupplierFoodAssignment { PackageId = targetPackageId, Id = Guid.NewGuid() },
            new SupplierFoodAssignment { PackageId = targetPackageId, Id = Guid.NewGuid() },
            new SupplierFoodAssignment { PackageId = otherPackageId, Id = Guid.NewGuid() } // This should not be returned
        };

        _dbContext.SupplierFoodAssignments.AddRange(assignments);
        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _supplierFoodAssignmentRepository.GetByPackage(targetPackageId);

        // Assert
        results.Should().HaveCount(2)
            .And.OnlyContain(a => a.PackageId == targetPackageId);
    }
    
    [Fact]
    public async Task ToPaginationByPartner_ReturnsPaginatedResults()
    {
        var packageId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();  // Ensure supplierId matches the expected in query logic
        var assignment = new SupplierFoodAssignment 
        {
            Id = Guid.NewGuid(),  // This ID should be the first in its group if that matters
            PackageId = packageId,
            SupplierCommissionRate = new SupplierCommissionRate 
            {
                SupplierId = supplierId,  // Important for grouping logic
                Supplier = new Supplier
                {
                    Name = string.Empty,
                    Address = string.Empty,
                    PhoneNumber = string.Empty
                } // Ensure all necessary navigation properties are initialized
            },
            Package = new Package 
            { 
                Id = packageId,
                DailyOrder = new DailyOrder 
                { 
                    MealSubscription = new MealSubscription 
                    { 
                        Meal = new Meal
                        {
                            MealType = string.Empty
                        }
                    } 
                } 
            }
        };

        _dbContext.SupplierFoodAssignments.Add(assignment);
        await _dbContext.SaveChangesAsync();

        // Test the pagination with known IDs
        var result = await _supplierFoodAssignmentRepository.ToPaginationByPartner(new List<Guid> { packageId }, 0, 1);

        result.Items.Should().ContainSingle();
        result.TotalItemsCount.Should().Be(1);
        result.Items.First().Id.Should().Be(assignment.Id);
    }
    
    [Fact]
    public async Task ToPaginationBySupplier_ReturnsPaginatedResults()
    {
        var supplierId = Guid.NewGuid();
        var packageId = Guid.NewGuid();
        var commissionRateId = Guid.NewGuid();
        _dbContext.SupplierFoodAssignments.Add(new SupplierFoodAssignment 
        {
            PackageId = packageId,
            SupplierCommissionRate = new SupplierCommissionRate 
            {
                Id = commissionRateId,
                SupplierId = supplierId
            }
        });
        await _dbContext.SaveChangesAsync();

        var result = await _supplierFoodAssignmentRepository.ToPaginationBySupplier(new List<Guid> { commissionRateId }, 0, 1);

        result.Items.Should().ContainSingle();
        result.TotalItemsCount.Should().Be(1);
        result.PageIndex.Should().Be(0);
    }
    
    
    [Fact]
    public async Task GetByForSuperAdmin_FiltersCorrectly()
    {
        var fromDate = new DateOnly(2023, 1, 1);
        var toDate = new DateOnly(2023, 1, 10);
        var assignment = new SupplierFoodAssignment
        {
            Status = SupplierFoodAssignmentStatus.Confirmed,
            Package = new Package
            {
                DailyOrder = new DailyOrder { BookingDate = new DateOnly(2023,01,05) }
            }
        };

        _dbContext.SupplierFoodAssignments.Add(assignment);
        await _dbContext.SaveChangesAsync();

        var result = await _supplierFoodAssignmentRepository.GetByForSuperAdmin(fromDate, toDate);

        result.Should().ContainSingle();
        result.First().Status.Should().Be(SupplierFoodAssignmentStatus.Confirmed);
    }
}