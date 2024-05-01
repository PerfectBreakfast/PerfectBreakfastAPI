using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class SupplierCommissionRateControllerTest : SetupTest
{
    private readonly SupplierCommissionRateController _supplierCommissionRateController;

    public SupplierCommissionRateControllerTest()
    {
        _supplierCommissionRateController =
            new SupplierCommissionRateController(_supplierCommissionRateServiceMock.Object);
    }
    
    [Fact]
    public async Task GetSupplierCommissionRate_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<SupplierCommissionRateRespone> { Payload = new SupplierCommissionRateRespone(), IsError = false };
        _supplierCommissionRateServiceMock.Setup(x => x.GetSupplierCommissionRateId(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.GetSupplierCommissionRate(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetSupplierCommissionRate_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<SupplierCommissionRateRespone> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Supplier commission rate not found." } } };
        _supplierCommissionRateServiceMock.Setup(x => x.GetSupplierCommissionRateId(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.GetSupplierCommissionRate(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetSupplierMoreFood_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<FoodResponse>> { Payload = new List<FoodResponse>(), IsError = false };
        _supplierCommissionRateServiceMock.Setup(x => x.GetSupplierMoreFood(supplierId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.GetSupplierMoreFood(supplierId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetSupplierMoreFood_ReturnsBadRequest_WhenServiceErrors()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<FoodResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No food found for the supplier." } } };
        _supplierCommissionRateServiceMock.Setup(x => x.GetSupplierMoreFood(supplierId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.GetSupplierMoreFood(supplierId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateSupplierCommissionRate_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var request = new CreateSupplierCommissionRateRequest();
        var mockResponse = new OperationResult<List<SupplierCommissionRateRespone>> { Payload = new List<SupplierCommissionRateRespone>(), IsError = false };
        _supplierCommissionRateServiceMock.Setup(x => x.CreateSupplierCommissionRate(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.CreateSupplierCommissionRate(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateSupplierCommissionRate_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var request = new CreateSupplierCommissionRateRequest();
        var mockResponse = new OperationResult<List<SupplierCommissionRateRespone>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to create supplier commission rate." } } };
        _supplierCommissionRateServiceMock.Setup(x => x.CreateSupplierCommissionRate(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.CreateSupplierCommissionRate(request);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task UpdateSupplierCommissionRate_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateSupplierCommissionRateRequest(1);
        var mockResponse = new OperationResult<SupplierCommissionRateRespone> { Payload = new SupplierCommissionRateRespone(), IsError = false };
        _supplierCommissionRateServiceMock.Setup(x => x.UpdateSupplierCommissionRate(id, request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.UpdateSupplierCommissionRate(id, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateSupplierCommissionRate_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateSupplierCommissionRateRequest(1);
        var mockResponse = new OperationResult<SupplierCommissionRateRespone> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid request." } } };
        _supplierCommissionRateServiceMock.Setup(x => x.UpdateSupplierCommissionRate(id, request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.UpdateSupplierCommissionRate(id, request);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<SupplierCommissionRateRespone> { Payload = new SupplierCommissionRateRespone(), IsError = false };
        _supplierCommissionRateServiceMock.Setup(x => x.Delete(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.Delete(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<SupplierCommissionRateRespone> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to delete supplier commission rate." } } };
        _supplierCommissionRateServiceMock.Setup(x => x.Delete(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierCommissionRateController.Delete(id);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

}