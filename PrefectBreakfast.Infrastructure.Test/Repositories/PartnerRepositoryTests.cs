using System.Linq.Expressions;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class PartnerRepositoryTests : SetupTest
{
    private readonly IPartnerRepository _partnerRepository;

    public PartnerRepositoryTests()
    {
        _partnerRepository = new PartnerRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetPartnerDetail_ReturnsPartnerWithDetails()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = new Partner
        {
            Id = partnerId,
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty,
            CommissionRate = 0,
            Companies = new List<Company>
            {
                new Company
                {
                    IsDeleted = false,
                    Name = "",
                    PhoneNumber = "",
                    Email = "",
                    Address = ""
                }
            },
            SupplyAssignments = new List<SupplyAssignment>
            {
                new SupplyAssignment
                {
                    Supplier = new Supplier
                    {
                        IsDeleted = false,
                        Name = string.Empty,
                        Address = string.Empty,
                        PhoneNumber = string.Empty
                    }
                }
            },
            
        };
        _dbContext.Partners.Add(partner);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _partnerRepository.GetPartnerDetail(partnerId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(partnerId);
        result.Companies.Should().HaveCount(1);
        result.SupplyAssignments.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task GetPartnerById_UsesDynamicIncludes()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = new Partner
        {
            Id = partnerId,
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty,
            CommissionRate = 0
        };
        _dbContext.Partners.Add(partner);
        await _dbContext.SaveChangesAsync();

        Expression<Func<Partner, object>> include = p => p.Companies;

        // Act
        var result = await _partnerRepository.GetPartnerById(partnerId, include);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(partnerId);
    }
}