using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Hangfire;
using MapsterMapper;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class OrderServiceTests : SetupTest
{
    private readonly IOrderService _orderService;

    public OrderServiceTests()
    {
        _orderService = new OrderService(_unitOfWorkMock.Object,
            _mapper,
            _claimServiceMock.Object,
            _payOsServiceMock.Object,
            _distributedCacheMock.Object,
            _backgroundJobClientMock.Object,
            _currentTimeMock.Object,
            _MomoServiceMock.Object);
    }

    [Fact]
    public async Task TestCreateOrder_Success_BankingPayment()
    {
        // Arrange
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new User());
        _unitOfWorkMock
            .Setup(x => x.DailyOrderRepository.FindSingleAsync(It.IsAny<Expression<Func<DailyOrder, bool>>>()))
            .Returns(Task.FromResult(new DailyOrder()));
        _unitOfWorkMock.Setup(x => x.ComboFoodRepository.GetFoodByComboId(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Food>() { new Food { Price = 10 } });
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(new Food { Price = 5, FoodStatus = FoodStatus.Retail }));
        _unitOfWorkMock.Setup(x =>
                x.PaymentMethodRepository.FindSingleAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
            .Returns(Task.FromResult(new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "BANKING"
            }));
        _unitOfWorkMock.Setup(x => x.OrderRepository.AddAsync(It.IsAny<Order>())).Returns(Task.FromResult(new Order()));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Returns(Task.FromResult(1));
        _payOsServiceMock.Setup(x => x.CreatePaymentLink(It.IsAny<Order>()))
            .Returns(Task.FromResult(new PaymentResponse { IsSuccess = true }));

        var orderRequest = new OrderRequest
        {
            MealSubscriptionId = Guid.NewGuid(),
            MenuDate = DateTime.UtcNow,
            OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { FoodId = Guid.NewGuid(), Quantity = 1 },
                new OrderDetailRequest { ComboId = Guid.NewGuid(), Quantity = 1 }
            },
            Payment = ConstantPaymentMethod.BANKING
        };

        // Act
        var operationResult = await _orderService.CreateOrder(orderRequest);

        // Assert
        Assert.NotNull(operationResult.Payload);
        operationResult.Payload.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task TestCreateOrder_Fails_NotFoundMealSubscription()
    {
        // Arrange
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new User());
        _unitOfWorkMock
            .Setup(x => x.DailyOrderRepository.FindSingleAsync(It.IsAny<Expression<Func<DailyOrder, bool>>>()))
            .Returns(Task.FromResult<DailyOrder>(null));

        var orderRequest = new OrderRequest
        {
            MealSubscriptionId = Guid.NewGuid(),
            MenuDate = DateTime.UtcNow,
            OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { FoodId = Guid.NewGuid(), Quantity = 1 }
            },
            Payment = ConstantPaymentMethod.BANKING
        };

        // Act
        var operationResult = await _orderService.CreateOrder(orderRequest);

        // Assert
        Assert.Equal(ErrorCode.NotFound, operationResult.Errors.FirstOrDefault().Code);
        _unitOfWorkMock.Verify(x => x.OrderRepository.AddAsync(It.IsAny<Order>()), Times.Never);
        _payOsServiceMock.Verify(x => x.CreatePaymentLink(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task TestCreateOrder_Fails_InvalidPaymentMethod()
    {
        // Arrange
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new User());
        _unitOfWorkMock
            .Setup(x => x.DailyOrderRepository.FindSingleAsync(It.IsAny<Expression<Func<DailyOrder, bool>>>()))
            .Returns(Task.FromResult(new DailyOrder()));


        var orderRequest = new OrderRequest
        {
            MealSubscriptionId = Guid.NewGuid(),
            MenuDate = DateTime.UtcNow,
            OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { FoodId = Guid.NewGuid(), Quantity = 1 }
            },
            Payment = "INVALID_PAYMENT"
        };

        // Act
        var operationResult = await _orderService.CreateOrder(orderRequest);

        // Assert
        Assert.Contains(operationResult.Errors,
            error => error.Code == ErrorCode.UnknownError); // Or a more specific error code
        _unitOfWorkMock.Verify(x => x.OrderRepository.AddAsync(It.IsAny<Order>()), Times.Never);
        _payOsServiceMock.Verify(x => x.CreatePaymentLink(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task TestCreateOrder_Success_MomoPayment()
    {
        // Arrange
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new User());
        _unitOfWorkMock
            .Setup(x => x.DailyOrderRepository.FindSingleAsync(It.IsAny<Expression<Func<DailyOrder, bool>>>()))
            .Returns(Task.FromResult(new DailyOrder()));
        _unitOfWorkMock.Setup(x => x.ComboFoodRepository.GetFoodByComboId(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Food>() { new Food { Price = 10 } });
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(new Food { Price = 5, FoodStatus = FoodStatus.Retail }));
        _unitOfWorkMock.Setup(x =>
                x.PaymentMethodRepository.FindSingleAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
            .Returns(Task.FromResult(new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "MOMO"
            }));
        _unitOfWorkMock.Setup(x => x.OrderRepository.AddAsync(It.IsAny<Order>())).Returns(Task.FromResult(new Order()));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Returns(Task.FromResult(1));
        _MomoServiceMock.Setup(x => x.CreateMomoPaymentLink(It.IsAny<Order>()))
            .Returns(Task.FromResult(new PaymentResponse { IsSuccess = true }));

        var orderRequest = new OrderRequest
        {
            MealSubscriptionId = Guid.NewGuid(),
            MenuDate = DateTime.UtcNow,
            OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { FoodId = Guid.NewGuid(), Quantity = 1 },
                new OrderDetailRequest { ComboId = Guid.NewGuid(), Quantity = 1 }
            },
            Payment = ConstantPaymentMethod.MOMO
        };

        // Act
        var operationResult = await _orderService.CreateOrder(orderRequest);

        // Assert
        Assert.NotNull(operationResult.Payload);
        operationResult.Payload.IsSuccess.Should().BeTrue();
        _MomoServiceMock.Verify(x => x.CreateMomoPaymentLink(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task TestCreateOrder_Fails_SaveChangeFailure()
    {
        // Arrange
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new User());
        _unitOfWorkMock
            .Setup(x => x.DailyOrderRepository.FindSingleAsync(It.IsAny<Expression<Func<DailyOrder, bool>>>()))
            .Returns(Task.FromResult(new DailyOrder()));
        _unitOfWorkMock.Setup(x => x.ComboFoodRepository.GetFoodByComboId(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Food>() { new Food { Price = 10 } });
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(new Food { Price = 5, FoodStatus = FoodStatus.Retail }));
        _unitOfWorkMock.Setup(x =>
                x.PaymentMethodRepository.FindSingleAsync(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
            .Returns(Task.FromResult(new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "BANKING"
            }));
        _unitOfWorkMock.Setup(x => x.OrderRepository.AddAsync(It.IsAny<Order>())).Returns(Task.FromResult(new Order()));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Returns(Task.FromResult(0)); // Simulate failure
        _payOsServiceMock.Setup(x => x.CreatePaymentLink(It.IsAny<Order>()))
            .Returns(Task.FromResult(new PaymentResponse { IsSuccess = true }));

        var orderRequest = new OrderRequest
        {
            MealSubscriptionId = Guid.NewGuid(),
            MenuDate = DateTime.UtcNow,
            OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { FoodId = Guid.NewGuid(), Quantity = 1 },
                new OrderDetailRequest { ComboId = Guid.NewGuid(), Quantity = 1 }
            },
            Payment = ConstantPaymentMethod.BANKING
        };

        // Act
        var operationResult = await _orderService.CreateOrder(orderRequest);

        // Assert
        Assert.Contains(operationResult.Errors, error => error.Code == ErrorCode.ServerError);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once); // SaveChangesAsync was called
    }


    [Fact]
    public async Task TestGetOrders_Exception()
    {
        // Arrange
        var expectedExceptionMessage = "Fake exception message";
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetAllAsync()).Throws(new Exception(expectedExceptionMessage));

        // Act
        var operationResult = await _orderService.GetOrders();

        // Assert
        Assert.Equal(ErrorCode.UnknownError, operationResult.Errors[0].Code);
        Assert.Contains(operationResult.Errors[0].Message, expectedExceptionMessage);
    }

    [Fact]
    public async Task TestRemoveOrder_Exception()
    {
        // Arrange
        var expectedOrderId = Guid.NewGuid();
        var expectedExceptionMessage = "Fake exception message";
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(expectedOrderId))
            .Throws(new Exception(expectedExceptionMessage));

        // Act
        var operationResult = await _orderService.RemoveOrder(expectedOrderId);

        // Assert
        Assert.Equal(ErrorCode.UnknownError, operationResult.Errors[0].Code);
        Assert.Contains(operationResult.Errors[0].Message, expectedExceptionMessage);
        _unitOfWorkMock.Verify(x => x.OrderRepository.GetByIdAsync(expectedOrderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.OrderRepository.Remove(It.IsAny<Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
    }

    [Fact]
    public async Task TestDeleteOrder_Success()
    {
        // Arrange
        var expectedOrderId = Guid.NewGuid();
        var expectedOrder = new Order { Id = expectedOrderId };
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(expectedOrderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var operationResult = await _orderService.DeleteOrder(expectedOrderId);

        // Assert
        Assert.Null(operationResult.Payload);
        _unitOfWorkMock.Verify(x => x.OrderRepository.GetByIdAsync(expectedOrderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.OrderRepository.SoftRemove(expectedOrder), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
    }

    [Fact]
    public async Task TestDeleteOrder_NotFound()
    {
        // Arrange
        var expectedOrderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(expectedOrderId))
            .ThrowsAsync(new NotFoundIdException());

        // Act
        var operationResult = await _orderService.DeleteOrder(expectedOrderId);

        // Assert
        _unitOfWorkMock.Verify(x => x.OrderRepository.GetByIdAsync(expectedOrderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.OrderRepository.SoftRemove(It.IsAny<Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
    }

    [Fact]
    public async Task TestDeleteOrder_Exception()
    {
        // Arrange
        var expectedOrderId = Guid.NewGuid();
        var expectedExceptionMessage = "Fake exception message";
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(expectedOrderId))
            .Throws(new Exception(expectedExceptionMessage));

        // Act
        var operationResult = await _orderService.DeleteOrder(expectedOrderId);

        // Assert
        Assert.Equal(ErrorCode.UnknownError, operationResult.Errors[0].Code);
        Assert.Contains(operationResult.Errors[0].Message, expectedExceptionMessage);
        _unitOfWorkMock.Verify(x => x.OrderRepository.GetByIdAsync(expectedOrderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.OrderRepository.SoftRemove(It.IsAny<Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
    }

    [Fact]
    public async Task TestCancelOrder_Success()
    {
        // Arrange
        var expectedOrderCode = 12345;
        var expectedOrder = new Order { Id = Guid.NewGuid(), OrderStatus = OrderStatus.Pending };
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetOrderByOrderCode(expectedOrderCode))
            .Returns(Task.FromResult(expectedOrder));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1); // Simulate successful save
        var mockCache = new Mock<IDistributedCache>();

        // Act
        var operationResult = await _orderService.CancelOrder(expectedOrderCode);

        // Assert
        Assert.Null(operationResult.Payload); // No payment response expected on cancellation
        _unitOfWorkMock.Verify(x => x.OrderRepository.GetOrderByOrderCode(expectedOrderCode), Times.Once);
    }

    [Fact]
    public async Task TestCancelOrder_UnitOfWorkSaveChangesException()
    {
        // Arrange
        var expectedOrderCode = 12345;
        var expectedOrder = new Order { Id = Guid.NewGuid(), OrderStatus = OrderStatus.Pending };
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetOrderByOrderCode(expectedOrderCode))
            .Returns(Task.FromResult(expectedOrder));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Throws(new Exception("Fake exception message"));

        // Act
        var operationResult = await _orderService.CancelOrder(expectedOrderCode);

        // Assert
        _unitOfWorkMock.Verify(x => x.OrderRepository.GetOrderByOrderCode(expectedOrderCode), Times.Once);
        _unitOfWorkMock.Verify(x => x.OrderRepository.Update(It.IsAny<Order>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
    }
    
    /*[Fact]
    public async Task GetOrderByDailyOrder_ReturnsOrdersSuccessfully()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), TotalPrice = 100.00m, OrderStatus = OrderStatus.Paid, OrderCode = 12345 },
            // Additional orders can be added here
        };
        var dailyOrder = new DailyOrder { Orders = orders };

        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAndIncludeAsync(dailyOrderId, It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>()))
            .ReturnsAsync(dailyOrder);
        //_mapper.Map<List<OrderResponse>>(It.IsAny<List<Order>>()).Returns(orders.Select(o => new OrderResponse { Id = o.Id, TotalPrice = o.TotalPrice }).ToList());

        // Act
        var result = await _orderService.GetOrderByDailyOrder(dailyOrderId);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(orders.Count, result.Payload.Count);
    }*/
    

    [Fact]
    public async Task GetOrderByDailyOrder_HandlesExceptions()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByIdAndIncludeAsync(dailyOrderId, It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _orderService.GetOrderByDailyOrder(dailyOrderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Unexpected error"));
    }
    
    
    [Fact]
    public async Task UpdateOrder_UpdatesSuccessfully()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var existingOrder = new Order { Id = orderId, OrderStatus = OrderStatus.Pending };
        var updateRequest = new UpdateOrderRequest { OrderStatus = OrderStatus.Complete };

        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(orderId))
            .ReturnsAsync(existingOrder);
        _unitOfWorkMock.Setup(x => x.OrderRepository.Update(It.IsAny<Order>()));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _orderService.UpdateOrder(orderId, updateRequest);

        // Assert
        Assert.False(result.IsError);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        Assert.Equal(OrderStatus.Complete, existingOrder.OrderStatus);
    }

    [Fact]
    public async Task UpdateOrder_OrderNotFound_ReturnsError()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(orderId))
            .ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _orderService.UpdateOrder(orderId, new UpdateOrderRequest());

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Id is not exist", result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdateOrder_HandlesException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(orderId))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _orderService.UpdateOrder(orderId, new UpdateOrderRequest());

        // Assert
        Assert.True(result.IsError);
        Assert.Contains("Unexpected error", result.Errors.First().Message);
    }
    
    [Fact]
    public async Task GetOrderHistory_ReturnsOrdersWithDetailsSuccessfully()
    {
        // Arrange
        var pageNumber = 1;
        var userId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new Order {
                Id = Guid.NewGuid(), 
                TotalPrice = 100.00m, 
                OrderStatus = OrderStatus.Paid, 
                OrderCode = 12345,
                Worker = new User { Name = "Worker1" },
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { Combo = new Combo { Name = "Combo1" } },
                    new OrderDetail { Food = new Food { Name = "Food1" } }
                },
                DailyOrder = new DailyOrder {
                    MealSubscription = new MealSubscription { Meal = new Meal { MealType = "Lunch" } }
                }
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderHistory(userId, pageNumber * 5, It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>()))
            .ReturnsAsync(orders);
        //_mapper.Map<List<OrderHistoryResponse>>(orders).Returns(orders.Select(o => new OrderHistoryResponse(o.Id, o.Note, o.TotalPrice, o.OrderStatus.ToString(), o.OrderCode, o.CreationDate, o.OrderDetails.Count, o.Worker?.Company?.Name ?? "", DateOnly.FromDateTime(o.CreationDate), o.DailyOrder.MealSubscription.Meal.MealType)).ToList());

        // Act
        var result = await _orderService.GetOrderHistory(pageNumber);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(orders.Count, result.Payload.Count);
        Assert.All(result.Payload, item => Assert.NotEmpty(item.Meal));  // Checking that Meal data is loaded
    }

    [Fact]
    public async Task GetOrderHistory_HandlesException()
    {
        // Arrange
        var pageNumber = 1;
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderHistory(userId, pageNumber * 5, It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _orderService.GetOrderHistory(pageNumber);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task GetOrderHistoryByDeliveryStaff_ReturnsOrdersSuccessfully()
    {
        // Arrange
        var pageNumber = 1;
        var expectedPageNumber = pageNumber * 5; // since pageNumber *= 5;
        var userId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new Order {
                Id = Guid.NewGuid(), 
                TotalPrice = 200.00m, 
                OrderStatus = OrderStatus.Paid, 
                OrderCode = 67890,
                DailyOrder = new DailyOrder {
                    MealSubscription = new MealSubscription {
                        Meal = new Meal { MealType = "Dinner" }
                    }
                },
                Worker = new User { Name = "Delivery Staff", Company = new Company
                    {
                        Name = "Delivery Co.",
                        PhoneNumber = "null",
                        Email = "null",
                        Address = "null"
                    }
                },
                OrderDetails = new List<OrderDetail> {
                    new OrderDetail { Combo = new Combo { Name = "Combo Special" } }
                }
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderHistoryByDeliveryStaff(userId, expectedPageNumber, It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>()))
            .ReturnsAsync(orders);
        //_mapper.Map<List<OrderHistoryResponse>>(orders).Returns(orders.Select(o => new OrderHistoryResponse(o.Id, o.Note, o.TotalPrice, o.OrderStatus.ToString(), o.OrderCode, o.CreationDate, o.OrderDetails.Count, o.Worker.Company.Name, DateOnly.FromDateTime(o.CreationDate), o.DailyOrder.MealSubscription.Meal.MealType)).ToList());

        // Act
        var result = await _orderService.GetOrderHistoryByDeliveryStaff(pageNumber);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Single(result.Payload);
        Assert.Equal("Delivery Co.", result.Payload.First().CompanyName);
        //Assert.Equal("Combo Special", result.Payload.First().OrderDetails.First()?.Name);
    }

    [Fact]
    public async Task GetOrderHistoryByDeliveryStaff_HandlesException()
    {
        // Arrange
        var pageNumber = 1;
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderHistoryByDeliveryStaff(userId, pageNumber * 5, It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _orderService.GetOrderHistoryByDeliveryStaff(pageNumber);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }
    
    
    [Fact]
    public async Task CompleteOrder_FailsIfOrderNotPaidOrNotDelivering()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            OrderStatus = OrderStatus.Pending, // Not paid
            DailyOrder = new DailyOrder
            {
                Status = DailyOrderStatus.Waiting  // Not delivering
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(orderId, It.IsAny<Expression<Func<Order, object>>>()))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.CompleteOrder(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Thời gian giao hàng không phù hợp! Waiting", result.Errors.First().Message);
    }

    [Fact]
    public async Task CompleteOrder_FailsIfUserNotAssignedToOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            OrderStatus = OrderStatus.Paid,
            DailyOrder = new DailyOrder
            {
                Status = DailyOrderStatus.Delivering
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(orderId, It.IsAny<Expression<Func<Order, object>>>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(x => x.ShippingOrderRepository.GetShippingOrderByDailyOrderId(userId, order.DailyOrder.Id))
            .ReturnsAsync((ShippingOrder)null); // User is not assigned

        // Act
        var result = await _orderService.CompleteOrder(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Bạn không được giao nhiệm vụ cho đơn hàng này!", result.Errors.First().Message);
    }

    [Fact]
    public async Task CompleteOrder_HandlesException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAsync(orderId, It.IsAny<Expression<Func<Order, object>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _orderService.CompleteOrder(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains("Database error", result.Errors.First().Message);
    }
    
    // [Fact]
    // public async Task GetOrder_ReturnsOrderWithDetailsSuccessfully()
    // {
    //     // Arrange
    //     var orderId = Guid.NewGuid();
    //     var userId = Guid.NewGuid();
    //     var company = new Company
    //     {
    //         Id = Guid.NewGuid(),
    //         Name = "Test Company",
    //         PhoneNumber = "null",
    //         Email = "null",
    //         Address = "null"
    //     };
    //     var order = new Order
    //     {
    //         Id = orderId,
    //         Worker = new User { Id = userId, Company = company },
    //         WorkerId = userId,
    //         DailyOrder = new DailyOrder {
    //             BookingDate = DateOnly.FromDateTime(DateTime.UtcNow),
    //             MealSubscription = new MealSubscription { Meal = new Meal { MealType = "Lunch" } }
    //         },
    //         PaymentMethod = new PaymentMethod { Name = "Credit Card" },
    //         OrderDetails = new List<OrderDetail> {
    //             new OrderDetail {
    //                 Combo = new Combo { Name = "Lunch Special", Image = "image.jpg", ComboFoods = new List<ComboFood>() },
    //                 Quantity = 2,
    //                 UnitPrice = 15.00m
    //             }
    //         }
    //     };
    //
    //     _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
    //     _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAndIncludeAsync(orderId, It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>()))
    //         .ReturnsAsync(order);
    //     _unitOfWorkMock.Setup(x => x.CompanyRepository.GetByIdAsync(company.Id))
    //         .ReturnsAsync(company);
    //     //_mapper.Setup(m => m.Map<OrderResponse>(order)).Returns(new OrderResponse());
    //
    //     // Act
    //     var result = await _orderService.GetOrder(orderId);
    //
    //     // Assert
    //     Assert.False(result.IsError);
    //     Assert.NotNull(result.Payload);
    //     Assert.Equal("Test Company", result.Payload.Company.Name);
    // }

    [Fact]
    public async Task GetOrder_HandlesException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var exceptionMessage = "Database error";
        _unitOfWorkMock.Setup(x => x.OrderRepository.GetByIdAndIncludeAsync(orderId, It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>(), It.IsAny<IncludeInfo<Order>>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _orderService.GetOrder(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains(exceptionMessage));
    }

    [Fact]
    public async Task GetLinkContinuePayment_ReturnsPaymentLinkSuccessfully()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var cachedData = JsonSerializer.Serialize(new PaymentResponse
        {
            PaymentUrl = "http://example.com/payment",
            QrCode = "qrCodeData",
            DeepLink = "myApp://payment",
            IsSuccess = true
        });

        _distributedCacheMock.Setup(x => x.GetAsync($"order-{orderId}", default))
            .ReturnsAsync(Encoding.UTF8.GetBytes(cachedData));

        // Act
        var result = await _orderService.GetLinkContinuePayment(orderId);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal("http://example.com/payment", result.Payload.PaymentUrl);
        Assert.Equal("qrCodeData", result.Payload.QrCode);
        Assert.Equal("myApp://payment", result.Payload.DeepLink);
        Assert.True(result.Payload.IsSuccess);
    }

    [Fact]
    public async Task GetLinkContinuePayment_ReturnsNotFoundWhenNoData()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _distributedCacheMock.Setup(x => x.GetAsync($"order-{orderId}", default))
            .ReturnsAsync((byte[])null);

        // Act
        var result = await _orderService.GetLinkContinuePayment(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Không tồn tại phương phức thanh toán"));
    }

    [Fact]
    public async Task GetLinkContinuePayment_HandlesException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var exceptionMessage = "Cache service failure";
        _distributedCacheMock.Setup(x => x.GetAsync($"order-{orderId}", default))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _orderService.GetLinkContinuePayment(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains(exceptionMessage));
    }
}