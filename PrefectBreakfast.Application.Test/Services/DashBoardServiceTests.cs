using Moq;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class DashBoardServiceTests : SetupTest
{
    private readonly IDashboardService _dashboardService;

    public DashBoardServiceTests()
    {
        _dashboardService = new DashboardService(_unitOfWorkMock.Object, _currentTimeMock.Object);
    }
    
    [Fact]
    public async Task GetTop10CompanyOrder_ReturnsSuccessfully()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var fakeOrders = new List<Order>
        {
            new Order {
                Id = Guid.NewGuid(),
                DailyOrder = new DailyOrder {
                    MealSubscription = new MealSubscription {
                        Company = new Company { Id = Guid.NewGuid(), Name = "Company A", PhoneNumber = "1234567890", Email = "info@companya.com", Address = "1234 Main St" }
                    }
                }
            },
            new Order {
                Id = Guid.NewGuid(),
                DailyOrder = new DailyOrder {
                    MealSubscription = new MealSubscription {
                        Company = new Company { Id = Guid.NewGuid(), Name = "Company B", PhoneNumber = "0987654321", Email = "contact@companyb.com", Address = "4321 Side St" }
                    }
                }
            }
        };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderByDateForCompany(fromDate, toDate))
            .ReturnsAsync(fakeOrders);
        _currentTimeMock.Setup(c => c.GetCurrentTime()).Returns(DateTime.UtcNow);

        // Act
        var result = await _dashboardService.GetTop10CompanyOrder(null, null);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(2, result.Payload.Count); // Assuming top 10 and there are at least two different companies in the result
    }

    [Fact]
    public async Task GetTop10CompanyOrder_ReturnsErrorForInvalidDateRange()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)); // Invalid range

        _currentTimeMock.Setup(c => c.GetCurrentTime()).Returns(DateTime.UtcNow);

        // Act
        var result = await _dashboardService.GetTop10CompanyOrder(fromDate, toDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("'Từ ngày' phải nhỏ hơn với 'đến ngày'"));
    }

    [Fact]
    public async Task GetTop10CompanyOrder_HandlesException()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderByDateForCompany(fromDate, toDate))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _dashboardService.GetTop10CompanyOrder(fromDate, toDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task OrderAmountByDateStatistic_ReturnsStatisticsSuccessfully()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var orders = new List<Order>
        {
            new Order { CreationDate = DateTime.UtcNow.AddDays(-1) },
            new Order { CreationDate = DateTime.UtcNow.AddDays(-1) },
            new Order { CreationDate = DateTime.UtcNow.AddDays(-2) }
        };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderByDate(fromDate, toDate))
            .ReturnsAsync(orders);
        _currentTimeMock.Setup(c => c.GetCurrentTime()).Returns(DateTime.UtcNow);

        // Act
        var result = await _dashboardService.OrderAmountByDateStatistic(null, null);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(2, result.Payload.Count); // Two different days
        Assert.Equal(2, result.Payload.FirstOrDefault(d => d.Date == DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)))?.OrderAmount);
    }

    [Fact]
    public async Task OrderAmountByDateStatistic_InvalidDateRange_ReturnsError()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));

        // Act
        var result = await _dashboardService.OrderAmountByDateStatistic(fromDate, toDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("'Từ ngày' phải nhỏ hơn với 'đến ngày'"));
    }

    [Fact]
    public async Task OrderAmountByDateStatistic_HandlesException()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderByDate(fromDate, toDate))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _dashboardService.OrderAmountByDateStatistic(fromDate, toDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task OrderStatistic_ReturnsStatisticsSuccessfully()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var orders = new List<Order>
        {
            new Order { OrderStatus = OrderStatus.Complete },
            new Order { OrderStatus = OrderStatus.Complete },
            new Order { OrderStatus = OrderStatus.CancelledForOverdue },
            new Order { OrderStatus = OrderStatus.Pending }
        };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderByDate(fromDate, toDate))
            .ReturnsAsync(orders);
        _currentTimeMock.Setup(c => c.GetCurrentTime()).Returns(DateTime.UtcNow);

        // Act
        var result = await _dashboardService.OrderStatistic(fromDate, toDate);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(50.0, result.Payload.CompletePercent);
        Assert.Equal(25.0, result.Payload.CancelledForOverduePercent);
    }

    [Fact]
    public async Task OrderStatistic_InvalidDateRange_ReturnsError()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));

        // Act
        var result = await _dashboardService.OrderStatistic(fromDate, toDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("'Từ ngày' phải nhỏ hơn với 'đến ngày'"));
    }

    [Fact]
    public async Task OrderStatistic_HandlesException()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderByDate(fromDate, toDate))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _dashboardService.OrderStatistic(fromDate, toDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task GetTopComboAndFood_ReturnsTopItemsSuccessfully()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var orders = new List<Order>
        {
            new Order {
                OrderDetails = new List<OrderDetail> {
                    new OrderDetail { Combo = new Combo { Id = Guid.NewGuid(), Name = "Combo A", Image = "imageA.jpg", ComboFoods = new List<ComboFood> { new ComboFood { Food = new Food { Price = 10 } } } }, Quantity = 3 },
                    new OrderDetail { Food = new Food { Id = Guid.NewGuid(), Name = "Food B", Image = "imageB.jpg", Price = 5 }, Quantity = 2 }
                }
            }
        };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderByDateForComboAndFood(fromDate, toDate))
            .ReturnsAsync(orders);
        _currentTimeMock.Setup(c => c.GetCurrentTime()).Returns(DateTime.UtcNow);

        // Act
        var result = await _dashboardService.GetTopComboAndFood(fromDate, toDate);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(2, result.Payload.Count); // Should find two items (one combo, one food)
        Assert.Equal(3, result.Payload.First().Quantity);
        Assert.Equal("Combo A", result.Payload.First().Name);
    }

    [Fact]
    public async Task GetTopComboAndFood_InvalidDateRange_ReturnsError()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));

        // Act
        var result = await _dashboardService.GetTopComboAndFood(fromDate, toDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("'Từ ngày' phải nhỏ hơn với 'đến ngày'"));
    }

    [Fact]
    public async Task GetTopComboAndFood_HandlesException()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderByDateForComboAndFood(fromDate, toDate))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _dashboardService.GetTopComboAndFood(fromDate, toDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }
}