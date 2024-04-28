using System.Linq.Expressions;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class ShippingOrderServiceTest : SetupTest
{
    private readonly IShippingOrderService _shippingOrderService;

    public ShippingOrderServiceTest()
    {
        _shippingOrderService = new ShippingOrderService(_unitOfWorkMock.Object, _mapper,_claimServiceMock.Object,_currentTimeMock.Object);
    }
    
    [Fact]
    public async Task CreateShippingOrder_ReturnsSuccess_WithValidRequest()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var shipperIds = new List<Guid?>() { Guid.NewGuid() };
        var requestModel = new CreateShippingOrderRequest(dailyOrderId, shipperIds);

        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId, x => x.MealSubscription!))
            .ReturnsAsync(new DailyOrder { Status = DailyOrderStatus.Processing });

        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.ExistsWithDailyOrderAndShippers(dailyOrderId, shipperIds))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.AddRangeAsync(It.IsAny<List<ShippingOrder>>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _shippingOrderService.CreateShippingOrder(requestModel);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateShippingOrder_ReturnsError_OnInvalidDailyOrderStatus()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var shipperIds = new List<Guid?>() { Guid.NewGuid() };
        var requestModel = new CreateShippingOrderRequest(dailyOrderId, shipperIds);

        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId, x => x.MealSubscription!))
            .ReturnsAsync(new DailyOrder { Status = DailyOrderStatus.Complete });

        // Act
        var result = await _shippingOrderService.CreateShippingOrder(requestModel);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.BadRequest &&
                                                  e.Message.Contains("Trạng thái đơn hàng không đúng"));
    }
    
    [Fact]
    public async Task CreateShippingOrder_ReturnsError_OnDuplicateShippingOrder()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var shipperIds = new List<Guid?>() { Guid.NewGuid() };
        var requestModel = new CreateShippingOrderRequest(dailyOrderId, shipperIds);

        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId, x => x.MealSubscription!))
            .ReturnsAsync(new DailyOrder { Status = DailyOrderStatus.Processing });

        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.ExistsWithDailyOrderAndShippers(dailyOrderId, shipperIds))
            .ReturnsAsync(true);

        // Act
        var result = await _shippingOrderService.CreateShippingOrder(requestModel);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.BadRequest &&
                                                  e.Message.Contains("A shipping order with the same DailyOrderId and one of the ShipperIds already exists."));
    }
    
    [Fact]
    public async Task GetShippingOrderTodayByDeliveryStaff_ReturnsSuccess_WithData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mockShippingOrders = new List<ShippingOrder>(); // Replace with sample data

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderTodayByShipperIdAndDate(userId, date))
            .ReturnsAsync(mockShippingOrders);

        /*_mapperMock.Setup(x => x.Map<List<ShippingOrderForShipperResponse>>(It.IsAny<List<ShippingOrder>>()))
            .Returns(new List<ShippingOrderForShipperResponse>()); // Map to your response type*/

        // Act
        var result = await _shippingOrderService.GetShippingOrderTodayByDeliveryStaff(DateTime.UtcNow);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull(); // Assert data is present (modify based on your response type)
    }
    
    [Fact]
    public async Task GetShippingOrderTodayByDeliveryStaff_ReturnsError_OnException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderTodayByShipperIdAndDate(userId, date))
            .ThrowsAsync(new Exception("Error retrieving shipping orders"));

        // Act
        var result = await _shippingOrderService.GetShippingOrderTodayByDeliveryStaff(DateTime.UtcNow);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Error retrieving shipping orders"));
    }
    
    [Fact]
    public async Task GetDeliveryStaffByDailyOrder_ReturnsSuccess_WithData()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockUsers = new List<User>(); // Replace with sample user data

        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetStaffByDailyOrder(dailyOrderId))
            .ReturnsAsync(mockUsers);

            /*_mapperMock.Setup(x => x.Map<List<UserResponse>>(It.IsAny<List<User>>()))
            .Returns(new List<UserResponse>()); // Map to your response type*/

        // Act
        var result = await _shippingOrderService.GetDeliveryStaffByDailyOrder(dailyOrderId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull(); // Assert data is present (modify based on your response type)
    }

    [Fact]
    public async Task GetDeliveryStaffByDailyOrder_ReturnsError_OnException()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();  

        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetStaffByDailyOrder(dailyOrderId))
            .ThrowsAsync(new Exception("Error retrieving delivery staff"));

        // Act
        var result = await _shippingOrderService.GetDeliveryStaffByDailyOrder(dailyOrderId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Error retrieving delivery staff"));
    }
    
    [Fact]
    public async Task EndShippingOrderByShipper_ReturnsSuccess_WithValidConditions()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockShippingOrders = new List<ShippingOrder>(); // Replace with sample data
        var mockDailyOrder = new DailyOrder
        {
            Status = DailyOrderStatus.Delivering,
            BookingDate = DateOnly.FromDateTime(DateTime.UtcNow.Date), // Use DateOnly constructor
            MealSubscription = new MealSubscription { EndTime = new TimeOnly(18, 0) } // Set end time for validation
        };
        var mockOrders = new List<Order>(); // Replace with sample data

        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId))
            .ReturnsAsync(mockShippingOrders);

        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId, x => x.MealSubscription!))
            .ReturnsAsync(mockDailyOrder);

        _currentTimeMock.Setup(x => x.GetCurrentTime()).Returns(DateTime.UtcNow.AddHours(19)); // Set current time after end time

        _unitOfWorkMock.Setup(u => u.OrderRepository.FindAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns(mockOrders.AsQueryable().BuildMock());

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _shippingOrderService.EndShippingOrderByShipper(dailyOrderId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeTrue();

        // Verify daily order status is updated to complete
        mockDailyOrder.Status.Should().Be(DailyOrderStatus.Complete);

        // Verify order statuses are updated to cancelled for overdue
        mockOrders.ForEach(x => x.OrderStatus.Should().Be(OrderStatus.CancelledForOverdue));

        // Verify shipping order statuses are updated to complete
        mockShippingOrders.ForEach(x => x.Status.Should().Be(ShippingStatus.Complete));
    }
    
        
    [Fact]
    public async Task EndShippingOrderByShipper_ReturnsError_OnException()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId))
            .ThrowsAsync(new Exception("Error getting shipping orders"));

        // Act
        var result = await _shippingOrderService.EndShippingOrderByShipper(dailyOrderId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Error getting shipping orders"));

        // Verify no updates are made
        _unitOfWorkMock.Verify(u => u.DailyOrderRepository.Update(It.IsAny<DailyOrder>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.OrderRepository.UpdateRange(It.IsAny<List<Order>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.ShippingOrderRepository.UpdateRange(It.IsAny<List<ShippingOrder>>()), Times.Never);
    }

    [Fact]
    public async Task EndShippingOrderByShipper_ReturnsError_OnSaveChangeFails()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockShippingOrders = new List<ShippingOrder>(); // Replace with sample data
        var mockDailyOrder = new DailyOrder
        {
            Status = DailyOrderStatus.Delivering,
            BookingDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            MealSubscription = new MealSubscription { EndTime = new TimeOnly(18, 0) } // Set end time for validation
        };
        var mockOrders = new List<Order>(); // Replace with sample data

        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId))
            .ReturnsAsync(mockShippingOrders);

        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId, x => x.MealSubscription!))
            .ReturnsAsync(mockDailyOrder);

        _currentTimeMock.Setup(x => x.GetCurrentTime()).Returns(DateTime.UtcNow.AddHours(19)); // Set current time after end time

        _unitOfWorkMock.Setup(u => u.OrderRepository.FindAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns(mockOrders.AsQueryable().BuildMock);

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(0); // Simulate save failure

        // Act
        var result = await _shippingOrderService.EndShippingOrderByShipper(dailyOrderId);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.ServerError &&
                                                  e.Message.Contains("lỗi trong quá trình lưu xuống db"));
    }
    
    [Fact]
    public async Task ConfirmShippingOrderByShipper_ReturnsSuccess_WithValidConditions()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var mockShippingOrders = new List<ShippingOrder>()
        {
            new ShippingOrder { ShipperId = userId, Status = ShippingStatus.Pending }
        };
        var mockDailyOrder = new DailyOrder { Status = DailyOrderStatus.Waiting };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId))
            .ReturnsAsync(mockShippingOrders);
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId))
            .ReturnsAsync(mockDailyOrder);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _shippingOrderService.ConfirmShippingOrderByShipper(dailyOrderId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeTrue();

        // Verify shipping order statuses are updated to confirmed
        mockShippingOrders.ForEach(x => x.Status.Should().Be(ShippingStatus.Confirm));

        // Verify daily order status is updated to delivering
        mockDailyOrder.Status.Should().Be(DailyOrderStatus.Delivering);
    }
    
    [Fact]
    public async Task ConfirmShippingOrderByShipper_ReturnsError_OnInvalidDailyOrderStatus()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var mockShippingOrders = new List<ShippingOrder>() { new ShippingOrder { ShipperId = userId } };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId))
            .ReturnsAsync(mockShippingOrders);
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId))
            .ReturnsAsync(new DailyOrder { Status = DailyOrderStatus.Complete }); // Already completed

        // Act
        var result = await _shippingOrderService.ConfirmShippingOrderByShipper(dailyOrderId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.BadRequest &&
                                                  e.Message.Contains("Đơn đã hoàn thành rồi nhé"));

        // Verify no updates are made
        _unitOfWorkMock.Verify(u => u.ShippingOrderRepository.UpdateRange(It.IsAny<List<ShippingOrder>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.DailyOrderRepository.Update(It.IsAny<DailyOrder>()), Times.Never);
    }
    
    [Fact]
    public async Task ConfirmShippingOrderByShipper_ReturnsError_OnInvalidShipperId()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var mockShippingOrders = new List<ShippingOrder>() { new ShippingOrder { Status = ShippingStatus.Pending } };
        var mockDailyOrder = new DailyOrder { Status = DailyOrderStatus.Waiting };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId))
            .ReturnsAsync(mockShippingOrders);
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId))
            .ReturnsAsync(mockDailyOrder);

        // Act
        var result = await _shippingOrderService.ConfirmShippingOrderByShipper(dailyOrderId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.BadRequest &&
                                                  e.Message.Contains("Đơn hàng này không phải nhiệm vụ của bạn"));

        // Verify no updates are made
        _unitOfWorkMock.Verify(u => u.ShippingOrderRepository.UpdateRange(It.IsAny<List<ShippingOrder>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.DailyOrderRepository.Update(It.IsAny<DailyOrder>()), Times.Never);
    }
    
    [Fact]
    public async Task ConfirmShippingOrderByShipper_ReturnsError_OnInvalidShippingOrderStatus()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var mockShippingOrders = new List<ShippingOrder>()
        {
            new ShippingOrder { ShipperId = userId, Status = ShippingStatus.Complete } // Already completed
        };
        var mockDailyOrder = new DailyOrder { Status = DailyOrderStatus.Waiting };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId))
            .ReturnsAsync(mockShippingOrders);
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId))
            .ReturnsAsync(mockDailyOrder);

        // Act
        var result = await _shippingOrderService.ConfirmShippingOrderByShipper(dailyOrderId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.ServerError &&
                                                  e.Message.Contains("Có 1 đơn vận chuyển không phải trong trạng thái chờ xác nhận"));

        // Verify no updates are made
        _unitOfWorkMock.Verify(u => u.ShippingOrderRepository.UpdateRange(It.IsAny<List<ShippingOrder>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.DailyOrderRepository.Update(It.IsAny<DailyOrder>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmShippingOrderByShipper_ReturnsError_OnInvalidShippingOrderStatusMultiple()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var mockShippingOrders = new List<ShippingOrder>()
        {
            new ShippingOrder { ShipperId = userId, Status = ShippingStatus.Complete } // Already completed
        };
        var mockDailyOrder = new DailyOrder { Status = DailyOrderStatus.Waiting };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.ShippingOrderRepository.GetShippingOrderByDailyOrderId(dailyOrderId))
            .ReturnsAsync(mockShippingOrders);
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAsync(dailyOrderId))
            .ReturnsAsync(mockDailyOrder);

        // Act
        var result = await _shippingOrderService.ConfirmShippingOrderByShipper(dailyOrderId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.ServerError);
    }
}