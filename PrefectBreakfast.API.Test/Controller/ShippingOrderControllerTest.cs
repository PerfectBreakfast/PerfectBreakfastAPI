using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class ShippingOrderControllerTest : SetupTest
{
    private readonly ShippingOrderController _shippingOrderController;

    public ShippingOrderControllerTest()
    {
        _shippingOrderController = new ShippingOrderController(_shippingOrderServiceMock.Object);
    }
    
    [Fact]
    public async Task CreateShippingOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var requestModel = new CreateShippingOrderRequest(Guid.NewGuid(), new List<Guid?>());
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _shippingOrderServiceMock.Setup(x => x.CreateShippingOrder(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.CreateShippingOrder(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateShippingOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var requestModel = new CreateShippingOrderRequest(Guid.NewGuid(), new List<Guid?>());
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Failed to create shipping order." } } };
        _shippingOrderServiceMock.Setup(x => x.CreateShippingOrder(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.CreateShippingOrder(requestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetDailyOrderByDeliveryStaff_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var time = DateTime.UtcNow;
        var mockResponse = new OperationResult<List<ShippingOrderForShipperResponse>> { Payload = new List<ShippingOrderForShipperResponse>(), IsError = false };
        _shippingOrderServiceMock.Setup(x => x.GetShippingOrderTodayByDeliveryStaff(time)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.GetDailyOrderByDeliveryStaff(time);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDailyOrderByDeliveryStaff_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var time = DateTime.UtcNow;
        var mockResponse = new OperationResult<List<ShippingOrderForShipperResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No shipping orders found." } } };
        _shippingOrderServiceMock.Setup(x => x.GetShippingOrderTodayByDeliveryStaff(time)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.GetDailyOrderByDeliveryStaff(time);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task ConfirmStatusShippingOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _shippingOrderServiceMock.Setup(x => x.ConfirmShippingOrderByShipper(dailyOrderId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.ConfirmStatusShippingOrder(dailyOrderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task ConfirmStatusShippingOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Shipping order not found." } } };
        _shippingOrderServiceMock.Setup(x => x.ConfirmShippingOrderByShipper(dailyOrderId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.ConfirmStatusShippingOrder(dailyOrderId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task EndShippingOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _shippingOrderServiceMock.Setup(x => x.EndShippingOrderByShipper(dailyOrderId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.EndShippingOrder(dailyOrderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task EndShippingOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Failed to end shipping order." } } };
        _shippingOrderServiceMock.Setup(x => x.EndShippingOrderByShipper(dailyOrderId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.EndShippingOrder(dailyOrderId);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task GetDeliveryStaffByDailyOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<UserResponse>> { Payload = new List<UserResponse>(), IsError = false };
        _shippingOrderServiceMock.Setup(x => x.GetDeliveryStaffByDailyOrder(dailyOrderId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.GetDeliveryStaffByDailyOrder(dailyOrderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDeliveryStaffByDailyOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<UserResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No staff found for the daily order." } } };
        _shippingOrderServiceMock.Setup(x => x.GetDeliveryStaffByDailyOrder(dailyOrderId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _shippingOrderController.GetDeliveryStaffByDailyOrder(dailyOrderId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

}