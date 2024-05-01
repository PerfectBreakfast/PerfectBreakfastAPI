using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class OrderControllerTest : SetupTest
{
    private readonly OrderController _orderController;

    public OrderControllerTest()
    {
        _orderController = new OrderController(_orderServiceMock.Object);
    }
    
    [Fact]
    public async Task CreateOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var orderRequest = new OrderRequest();
        var mockResponse = new OperationResult<PaymentResponse>
        {
            Payload = new PaymentResponse(),
            IsError = false
        };
        _orderServiceMock.Setup(x => x.CreateOrder(orderRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.CreateOrder(orderRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var orderRequest = new OrderRequest();
        var mockResponse = new OperationResult<PaymentResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error processing request" } }
        };
        _orderServiceMock.Setup(x => x.CreateOrder(orderRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.CreateOrder(orderRequest);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<OrderResponse>
        {
            Payload = new OrderResponse(),
            IsError = false
        };
        _orderServiceMock.Setup(x => x.GetOrder(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrder(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<OrderResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Order not found" } }
        };
        _orderServiceMock.Setup(x => x.GetOrder(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrder(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<OrderResponse>
        {
            Payload = new OrderResponse(),
            IsError = false
        };
        _orderServiceMock.Setup(x => x.DeleteOrder(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.RemoveOrder(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task DeleteOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<OrderResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Order not found" } }
        };
        _orderServiceMock.Setup(x => x.DeleteOrder(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.RemoveOrder(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetLinkContinuePayment_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<PaymentResponse>
        {
            Payload = new PaymentResponse(), // Payload should include payment link details
            IsError = false
        };
        _orderServiceMock.Setup(x => x.GetLinkContinuePayment(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetLinkContinuePayment(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetLinkContinuePayment_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<PaymentResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Order not found" } }
        };
        _orderServiceMock.Setup(x => x.GetLinkContinuePayment(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetLinkContinuePayment(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task CancelOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        long orderCode = 1234567890;
        var mockResponse = new OperationResult<PaymentResponse>
        {
            Payload = new PaymentResponse(), // Assume cancellation confirmation details are in the payload
            IsError = false
        };
        _orderServiceMock.Setup(x => x.CancelOrder(orderCode)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.CancelOrder(orderCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CancelOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        long orderCode = 1234567890;
        var mockResponse = new OperationResult<PaymentResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Order not found" } }
        };
        _orderServiceMock.Setup(x => x.CancelOrder(orderCode)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.CancelOrder(orderCode);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOrderHistoryByCustomer_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        int pageNumber = 1;
        var mockResponse = new OperationResult<List<OrderHistoryResponse>>
        {
            Payload = new List<OrderHistoryResponse>(), // Assume some history data is returned
            IsError = false
        };
        _orderServiceMock.Setup(x => x.GetOrderHistory(pageNumber)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrderHistoryByCustomer(pageNumber);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetOrderHistoryByCustomer_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        int pageNumber = 1;
        var mockResponse = new OperationResult<List<OrderHistoryResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving order history" } }
        };
        _orderServiceMock.Setup(x => x.GetOrderHistory(pageNumber)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrderHistoryByCustomer(pageNumber);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOrders_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<OrderResponse>>
        {
            Payload = new List<OrderResponse>(),
            IsError = false
        };
        _orderServiceMock.Setup(x => x.GetOrders()).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetOrders_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<OrderResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving orders" } }
        };
        _orderServiceMock.Setup(x => x.GetOrders()).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrders();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOrderHistoryByDeliveryStaff_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        int pageNumber = 1;
        var mockResponse = new OperationResult<List<OrderHistoryResponse>>
        {
            Payload = new List<OrderHistoryResponse>(),
            IsError = false
        };
        _orderServiceMock.Setup(x => x.GetOrderHistoryByDeliveryStaff(pageNumber)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrderHistoryByDeliveryStaff(pageNumber);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetOrderHistoryByDeliveryStaff_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        int pageNumber = 1;
        var mockResponse = new OperationResult<List<OrderHistoryResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving history" } }
        };
        _orderServiceMock.Setup(x => x.GetOrderHistoryByDeliveryStaff(pageNumber)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrderHistoryByDeliveryStaff(pageNumber);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var updateRequest = new UpdateOrderRequest();
        var mockResponse = new OperationResult<OrderResponse>
        {
            Payload = new OrderResponse(),
            IsError = false
        };
        _orderServiceMock.Setup(x => x.UpdateOrder(id, updateRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.UpdateOrder(id, updateRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var updateRequest = new UpdateOrderRequest();
        var mockResponse = new OperationResult<OrderResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Order not found" } }
        };
        _orderServiceMock.Setup(x => x.UpdateOrder(id, updateRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.UpdateOrder(id, updateRequest);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOrderPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        int pageIndex = 1;
        int pageSize = 10;
        var mockResponse = new OperationResult<Pagination<OrderResponse>>
        {
            Payload = new Pagination<OrderResponse>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 100,
                Items = new List<OrderResponse>()
            },
            IsError = false
        };
        _orderServiceMock.Setup(x => x.GetOrderPaginationAsync(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrderPagination(pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetOrderPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        int pageIndex = 1;
        int pageSize = 10;
        var mockResponse = new OperationResult<Pagination<OrderResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving order pagination" } }
        };
        _orderServiceMock.Setup(x => x.GetOrderPaginationAsync(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrderPagination(pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task CompleteOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var mockResponse = new OperationResult<bool>
        {
            Payload = true,
            IsError = false
        };
        _orderServiceMock.Setup(x => x.CompleteOrder(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.CompleteOrder(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CompleteOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var mockResponse = new OperationResult<bool>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Could not complete order" } }
        };
        _orderServiceMock.Setup(x => x.CompleteOrder(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.CompleteOrder(id);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOrderByDailyOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid dailyOrderId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<OrderResponse>>
        {
            Payload = new List<OrderResponse>(),
            IsError = false
        };
        _orderServiceMock.Setup(x => x.GetOrderByDailyOrder(dailyOrderId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrderByDailyOrder(dailyOrderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetOrderByDailyOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid dailyOrderId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<OrderResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Daily order not found" } }
        };
        _orderServiceMock.Setup(x => x.GetOrderByDailyOrder(dailyOrderId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _orderController.GetOrderByDailyOrder(dailyOrderId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}