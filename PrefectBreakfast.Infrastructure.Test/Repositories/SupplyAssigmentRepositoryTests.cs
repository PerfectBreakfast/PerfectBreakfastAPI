using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class SupplyAssigmentRepositoryTests : SetupTest
{
    private readonly ISupplyAssigmentRepository _supplyAssigmentRepository;

    public SupplyAssigmentRepositoryTests()
    {
        _supplyAssigmentRepository = new SupplyAssigmentRepository(_dbContext);
    }
    
    [Fact]
    public async Task IsDuplicateAssignment_ReturnsTrue_WhenAssignmentExists()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var supplyAssignment = new SupplyAssignment
        {
            PartnerId = partnerId,
            SupplierId = supplierId
        };

        _dbContext.SupplyAssignments.Add(supplyAssignment);
        await _dbContext.SaveChangesAsync();

        // Act
        var isDuplicate = await _supplyAssigmentRepository.IsDuplicateAssignment(partnerId, supplierId);

        // Assert
        isDuplicate.Should().BeTrue();
    }
    
    [Fact]
    public async Task IsDuplicateAssignment_ReturnsFalse_WhenAssignmentDoesNotExist()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();

        // Ensure the database is empty or does not contain the specific assignment
        // No need to add data explicitly for this test

        // Act
        var isDuplicate = await _supplyAssigmentRepository.IsDuplicateAssignment(partnerId, supplierId);

        // Assert
        isDuplicate.Should().BeFalse();
    }
}