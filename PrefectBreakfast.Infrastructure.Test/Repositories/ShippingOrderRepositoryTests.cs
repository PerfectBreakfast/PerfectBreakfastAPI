using FluentAssertions;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class ShippingOrderRepositoryTests : SetupTest
{
    private readonly ShippingOrderRepository _shippingOrderRepository;

    public ShippingOrderRepositoryTests()
    {
        _shippingOrderRepository =
            new ShippingOrderRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetShippingOrderTodayByShipperIdAndDate_ReturnsRelevantOrders()
    {
        // Arrange
        var shipperId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var dailyOrder = new DailyOrder 
        {
            Id = Guid.NewGuid(),
            BookingDate = date,
            MealSubscription = new MealSubscription
            {
                Company = new Company
                {
                    Partner = new Partner
                    {
                        Name = string.Empty,
                        Address = string.Empty,
                        PhoneNumber = string.Empty,
                        CommissionRate = 0
                    },
                    Name = string.Empty,
                    PhoneNumber = string.Empty,
                    Email = string.Empty,
                    Address = string.Empty
                },
                Meal = new Meal
                {
                    MealType = string.Empty
                }
            }
        };

        var shippingOrder = new ShippingOrder
        {
            ShipperId = shipperId,
            DailyOrder = dailyOrder
        };

        _dbContext.ShippingOrders.Add(shippingOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shippingOrderRepository.GetShippingOrderTodayByShipperIdAndDate(shipperId, date);

        // Assert
        result.Should().ContainSingle();
        result.First().DailyOrder.MealSubscription.Company.Partner.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetShippingOrderByDailyOrderId_ReturnsCorrectOrder()
    {
        var shipperId = Guid.NewGuid();
        var dailyOrderId = Guid.NewGuid();
        var shippingOrder = new ShippingOrder { ShipperId = shipperId, DailyOrderId = dailyOrderId };
        _dbContext.ShippingOrders.Add(shippingOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shippingOrderRepository.GetShippingOrderByDailyOrderId(shipperId, dailyOrderId);

        // Assert
        result.Should().NotBeNull();
        result.DailyOrderId.Should().Be(dailyOrderId);
    }

    [Fact]
    public async Task ExistsWithDailyOrderAndShippers_ChecksExistenceCorrectly()
    {
        var dailyOrderId = Guid.NewGuid();
        var shipperIds = new List<Guid?> { Guid.NewGuid(), Guid.NewGuid() };
        var shippingOrder = new ShippingOrder { ShipperId = shipperIds.First().Value, DailyOrderId = dailyOrderId };
        _dbContext.ShippingOrders.Add(shippingOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _shippingOrderRepository.ExistsWithDailyOrderAndShippers(dailyOrderId, shipperIds);

        // Assert
        exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetStaffByDailyOrder_ReturnsStaffAssignedToDailyOrder()
    {
        var dailyOrderId = Guid.NewGuid();
        var shipper = new User { Id = Guid.NewGuid(), Name = string.Empty};
        var shippingOrder = new ShippingOrder { Shipper = shipper, DailyOrderId = dailyOrderId, Status = ShippingStatus.Pending};
        _dbContext.ShippingOrders.Add(shippingOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shippingOrderRepository.GetStaffByDailyOrder(dailyOrderId);

        // Assert
        result.Should().ContainSingle();
        //result.First().Should().Be(shipper);
    }
    
    [Fact]
    public async Task GetShippingOrderByDailyOrderId_ReturnsAllOrdersForDailyOrder()
    {
        var dailyOrderId = Guid.NewGuid();
        var shippingOrders = new List<ShippingOrder>
        {
            new ShippingOrder { DailyOrderId = dailyOrderId },
            new ShippingOrder { DailyOrderId = dailyOrderId }
        };

        _dbContext.ShippingOrders.AddRange(shippingOrders);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId);

        // Assert
        result.Should().HaveCount(2);
        result.All(so => so.DailyOrderId == dailyOrderId).Should().BeTrue();
    }
}