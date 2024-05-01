using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Request;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class PaymentMethodControllerTest : SetupTest
{
    private readonly PaymentMethodController _paymentMethodController;

    public PaymentMethodControllerTest()
    {
        _paymentMethodController = new PaymentMethodController(_paymentServiceMock.Object);
    }
    
     [Fact]
    public async Task GetPaymentMethods_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var mockResponse = new OperationResult<List<PaymentMethodResponse>> { Payload = new List<PaymentMethodResponse>(), IsError = false };
        _paymentServiceMock.Setup(s => s.GetPaymentMethods()).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.GetPaymentMethods();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetPaymentMethods_ReturnsError_WhenFailed()
    {
        // Arrange
        var mockResponse = new OperationResult<List<PaymentMethodResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Failed" } } };
        _paymentServiceMock.Setup(s => s.GetPaymentMethods()).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.GetPaymentMethods();

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetPaymentMethod_ReturnsOk_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<PaymentMethodResponse> { Payload = new PaymentMethodResponse(){Name = ""}, IsError = false };
        _paymentServiceMock.Setup(s => s.GetPaymentMethod(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.GetPaymentMethod(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetPaymentMethod_ReturnsNotFound_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<PaymentMethodResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Not found" } } };
        _paymentServiceMock.Setup(s => s.GetPaymentMethod(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.GetPaymentMethod(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreatePaymentMethod_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var request = new PaymentMethodRequest(){Name = ""};
        var mockResponse = new OperationResult<PaymentMethodResponse> { Payload = new  PaymentMethodResponse(){Name = ""}, IsError = false };
        _paymentServiceMock.Setup(s => s.CreatePaymentMethod(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await  _paymentMethodController.CreatePaymentMethod(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreatePaymentMethod_ReturnsBadRequest_WhenFailed()
    {
        // Arrange
        var request = new PaymentMethodRequest(){Name = ""};
        var mockResponse = new OperationResult<PaymentMethodResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Invalid data" } } };
        _paymentServiceMock.Setup(s => s.CreatePaymentMethod(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.CreatePaymentMethod(request);

        // Assert
        var badRequestResult = Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdatePaymentMethod_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new PaymentMethodRequest(){Name = ""};
        var mockResponse = new OperationResult<PaymentMethodResponse> { Payload = new PaymentMethodResponse(){Name = ""}, IsError = false };
        _paymentServiceMock.Setup(s => s.UpdatePaymentMethod(id, request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.UpdatePaymentMethod(id, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }
    
    [Fact]
    public async Task DeletePaymentMethod_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<PaymentMethodResponse> { IsError = false, Payload = new PaymentMethodResponse(){Name = ""} };
        _paymentServiceMock.Setup(s => s.DeletePaymentMethod(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.DeletePaymentMethod(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task DeletePaymentMethod_ReturnsError_WhenFailed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<PaymentMethodResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Invalid data" } } };
        _paymentServiceMock.Setup(s => s.DeletePaymentMethod(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.DeletePaymentMethod(id);

        // Assert
        var errorResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, errorResult.StatusCode);
    }

    [Fact]
    public async Task GetPaymentMethodPagination_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var mockResponse = new OperationResult<Pagination<PaymentMethodResponse>>
        {
            IsError = false,
            Payload = new Pagination<PaymentMethodResponse> { Items = new List<PaymentMethodResponse>(), TotalItemsCount = 10 }
        };
        _paymentServiceMock.Setup(s => s.GetPaymentMethodPaginationAsync(0, 10)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.GetPaymentMethodPagination(0, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<PaymentMethodResponse> { IsError = false, Payload = new PaymentMethodResponse(){Name = ""} };
        _paymentServiceMock.Setup(s => s.Delete(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.Delete(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsError_WhenFailed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<PaymentMethodResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Invalid data" } } };
        _paymentServiceMock.Setup(s => s.Delete(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _paymentMethodController.Delete(id);

        // Assert
        var errorResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, errorResult.StatusCode);
    }
}