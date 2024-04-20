using System.Linq.Expressions;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class DeliveryRepositoryTests : SetupTest
{
    private readonly IDeliveryRepository _deliveryRepository;

    public DeliveryRepositoryTests()
    {
        _deliveryRepository = new DeliveryRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetDeliveryById_ReturnsDeliveryWithDetails()
    {
        // Arrange
        var deliveryId = Guid.NewGuid();
        var delivery = new Delivery
        {
            Id = deliveryId,
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty,
            CommissionRate = 0,
            Companies = new List<Company?>
            {
                new Company
                {
                    Id = Guid.NewGuid(),
                    Email = string.Empty,
                    Name = "NAME",
                    Address = string.Empty,
                    PhoneNumber = string.Empty,
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Email = string.Empty,
                    Name = string.Empty,
                    Address = string.Empty,
                    PhoneNumber = string.Empty,
                },
            }
        };

        _dbContext.Deliveries.Add(delivery);
        await _dbContext.SaveChangesAsync();

        Expression<Func<Delivery, object>>[] includes = new Expression<Func<Delivery, object>>[]
        {
            d => d.Companies
        };

        // Act
        var result = await _deliveryRepository.GetDeliveryById(deliveryId, includes);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(deliveryId);
        result.Companies.Should().NotBeNull();
        result.Companies.First().Name.Should().Be("NAME");
    }
    
    [Fact]
    public async Task GetDeliveryById_ReturnsNull_IfDeliveryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid(); // Ensuring this ID does not exist in the database

        // Act
        var result = await _deliveryRepository.GetDeliveryById(nonExistentId);

        // Assert
        result.Should().BeNull();
    }
}