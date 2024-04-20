using System.Linq.Expressions;
using FluentAssertions;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class OrderRepositoryTests : SetupTest
{
    private readonly IOrderRepository _orderRepository;

    public OrderRepositoryTests()
    {
        _orderRepository = new OrderRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetOrderByDailyOrderId_ReturnsFilteredOrders()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var order1 = new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.Complete };
        var order2 = new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.Cancel };
        var order3 = new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.Pending }; // Should not be included
        var order4 = new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.Paid }; // Should not be included
        var order5 = new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.CancelledForOverdue }; // Should not be included
        _dbContext.Orders.AddRange(order1, order2, order3,order4,order5);
        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _orderRepository.GetOrderByDailyOrderId(dailyOrderId);

        // Assert
        results.Should().HaveCount(3).And.NotContain(o => o.OrderStatus == OrderStatus.Pending);
    }
    
    [Fact]
    public async Task AreAllOrdersCompleteForDailyOrder_ReturnsTrueWhenAllOrdersAreCompleteOrCanceled()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        _dbContext.Orders.Add(new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.Complete });
        _dbContext.Orders.Add(new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.CancelledForOverdue });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.AreAllOrdersCompleteForDailyOrder(dailyOrderId);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task AreAllOrdersCompleteForDailyOrder_ReturnsFalseWhenAllOrdersAnyPaid()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        _dbContext.Orders.Add(new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.Complete });
        _dbContext.Orders.Add(new Order { DailyOrderId = dailyOrderId, OrderStatus = OrderStatus.Paid });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.AreAllOrdersCompleteForDailyOrder(dailyOrderId);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task GetOrderHistory_ReturnsPagedResults()
    {
        var userId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new Order { WorkerId = userId, CreationDate = DateTime.UtcNow.AddDays(-1), Id = Guid.NewGuid()},
            new Order { WorkerId = userId, CreationDate = DateTime.UtcNow.AddDays(-2), Id = Guid.NewGuid()},
            new Order { WorkerId = userId, CreationDate = DateTime.UtcNow.AddDays(-3), Id = Guid.NewGuid()}
        };

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        var result = await _orderRepository.GetOrderHistory(userId, 2);

        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(o => o.CreationDate);
    }
    
    [Fact]
    public async Task GetOrderHistory_ReturnsPagedResultsWhenHasIncludeProperties()
    {
        var userId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new Order { WorkerId = userId, CreationDate = DateTime.UtcNow.AddDays(-1), Id = Guid.NewGuid(),OrderDetails = new List<OrderDetail?>
            {
                new OrderDetail
                {
                    Combo = null,
                    Food = new Food { Name = "Soda" }
                }
            }},
            new Order { WorkerId = userId, CreationDate = DateTime.UtcNow.AddDays(-1), Id = Guid.NewGuid(),OrderDetails = new List<OrderDetail?>
            {
                new OrderDetail
                {
                    Combo = null,
                    Food = new Food { Name = "cake" }
                }
            }},
            new Order { WorkerId = userId, CreationDate = DateTime.UtcNow.AddDays(-1), Id = Guid.NewGuid(),OrderDetails = new List<OrderDetail?>
            {
                new OrderDetail
                {
                    Combo = null,
                    Food = new Food { Name = "hehe" }
                }
            }},
        };

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        var orderInclude = new IncludeInfo<Order>
        {
            NavigationProperty = order => order.OrderDetails,
            ThenIncludes = new List<Expression<Func<object, object>>>
            {
                sp => ((OrderDetail)sp).Food
            }
        };
        var result = await _orderRepository.GetOrderHistory(userId, 2,orderInclude);

        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(o => o.CreationDate);
        // có thể kiểm tra thêm 
    }
    
    [Fact]
    public async Task GetOrderHistoryByDeliveryStaff_ReturnsFilteredOrders()
    {
        var userId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new Order { DeliveryStaffId = userId, CreationDate = DateTime.UtcNow.AddDays(-1), Id = Guid.NewGuid()},
            new Order { DeliveryStaffId = userId, CreationDate = DateTime.UtcNow.AddDays(-2), Id = Guid.NewGuid()},
            new Order { DeliveryStaffId = Guid.NewGuid(), CreationDate = DateTime.UtcNow.AddDays(-3), Id = Guid.NewGuid()} // Should not be included
        };

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        var result = await _orderRepository.GetOrderHistoryByDeliveryStaff(userId, 2);

        result.Should().HaveCount(2);
        result.Should().NotContain(o => o.DeliveryStaffId != userId);
    }
    
    [Fact]
    public async Task GetOrderHistoryByDeliveryStaff_ReturnsPagedResultsWhenHasIncludeProperties()
    {
        var userId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new Order { DeliveryStaffId = userId, CreationDate = DateTime.UtcNow.AddDays(-1), Id = Guid.NewGuid(),OrderDetails = new List<OrderDetail?>
            {
                new OrderDetail
                {
                    Combo = null,
                    Food = new Food { Name = "Soda" }
                }
            }},
            new Order { DeliveryStaffId = userId, CreationDate = DateTime.UtcNow.AddDays(-1), Id = Guid.NewGuid(),OrderDetails = new List<OrderDetail?>
            {
                new OrderDetail
                {
                    Combo = null,
                    Food = new Food { Name = "cake" }
                }
            }},
            new Order { DeliveryStaffId = userId, CreationDate = DateTime.UtcNow.AddDays(-3), Id = Guid.NewGuid(),OrderDetails = new List<OrderDetail?>
            {
                new OrderDetail
                {
                    Combo = null,
                    Food = new Food { Name = "hehe" }
                }
            }},
        };

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        var orderInclude = new IncludeInfo<Order>
        {
            NavigationProperty = order => order.OrderDetails,
            ThenIncludes = new List<Expression<Func<object, object>>>
            {
                sp => ((OrderDetail)sp).Food
            }
        };
        var result = await _orderRepository.GetOrderHistoryByDeliveryStaff(userId, 2,orderInclude);

        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(o => o.CreationDate);
        // có thể kiểm tra thêm 
    }
    
    [Fact]
    public async Task GetOrderByDate_ReturnsOrdersWithinDateRange()
    {
        var fromDate = new DateOnly(2022, 1, 1);
        var toDate = new DateOnly(2022, 1, 31);
        var orders = new List<Order>
        {
            new Order { CreationDate = new DateTime(2022, 1, 15), OrderStatus = OrderStatus.Complete, Id = Guid.NewGuid()},
            new Order { CreationDate = new DateTime(2022, 1, 20), OrderStatus = OrderStatus.CancelledForOverdue, Id = Guid.NewGuid()},
            new Order { CreationDate = new DateTime(2022, 2, 1), OrderStatus = OrderStatus.Complete, Id = Guid.NewGuid()} // Should not be included
        };

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        var result = await _orderRepository.GetOrderByDate(fromDate, toDate);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(o => o.CreationDate >= fromDate.ToDateTime(new TimeOnly()) && o.CreationDate <= toDate.ToDateTime(new TimeOnly()));
    }
    
    [Fact]
    public async Task GetOrderByDateForCompany_ReturnsOrdersWithinDateRange()
    {
        var fromDate = new DateOnly(2022, 1, 1);
        var toDate = new DateOnly(2022, 1, 31);
        var orders = new List<Order>
        {
            new Order { CreationDate = new DateTime(2022, 1, 15), OrderStatus = OrderStatus.Complete, Id = Guid.NewGuid()},
            new Order { CreationDate = new DateTime(2022, 1, 20), OrderStatus = OrderStatus.CancelledForOverdue, Id = Guid.NewGuid()},
            new Order { CreationDate = new DateTime(2022, 2, 1), OrderStatus = OrderStatus.Complete, Id = Guid.NewGuid()} // Should not be included
        };

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        var result = await _orderRepository.GetOrderByDateForCompany(fromDate, toDate);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(o => o.CreationDate >= fromDate.ToDateTime(new TimeOnly()) && o.CreationDate <= toDate.ToDateTime(new TimeOnly()));
    }
    
    [Fact]
    public async Task GetOrderByDateForComboAndFood_ReturnsOrdersWithinDateRangeWithDetails()
    {
        // Arrange
        var fromDate = new DateOnly(2023, 1, 10);
        var toDate = new DateOnly(2023, 1, 20);
        var orders = new List<Order>
        {
            new Order 
            {
                CreationDate = new DateTime(2023, 1, 15),
                OrderStatus = OrderStatus.Complete,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        Combo = new Combo 
                        { 
                            Name = "",
                            Content = "",
                            ComboFoods = new List<ComboFood> 
                            {
                                new ComboFood { Food = new Food { Name = "Pizza" } }
                            }
                        },
                        Food = new Food { Name = "Soda" }
                    }
                }
            },
            new Order 
            {
                CreationDate = new DateTime(2023, 1, 21), // Should not be included
                OrderStatus = OrderStatus.Complete,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        Food = new Food { Name = "Burger" }
                    }
                }
            }
        };

        await _dbContext.Orders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _orderRepository.GetOrderByDateForComboAndFood(fromDate, toDate);

        // Assert
        result.Should().ContainSingle();
        result.First().OrderDetails.First().Combo.ComboFoods.First().Food.Name.Should().Be("Pizza");
        result.First().OrderDetails.First().Food.Name.Should().Be("Soda");
        result.Should().NotContain(o => o.CreationDate.Date > toDate.ToDateTime(new TimeOnly()));
    }
    
    [Fact]
    public async Task GetOrderByOrderCode_ReturnsSingleOrder()
    {
        var orderCode = 1234567890L;
        var order = new Order { OrderCode = orderCode, Id = Guid.NewGuid() };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var result = await _orderRepository.GetOrderByOrderCode(orderCode);

        result.Should().NotBeNull();
        result.OrderCode.Should().Be(orderCode);
    }
}