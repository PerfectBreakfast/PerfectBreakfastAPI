using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Request;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class SupplyAssignmentControllerTest : SetupTest
{
    private readonly SupplyAssigmentController _supplyAssigmentController;

    public SupplyAssignmentControllerTest()
    {
        _supplyAssigmentController = new SupplyAssigmentController(_supplyAssigmentServiceMock.Object);
    }
    
    [Fact]
    public async Task GetSupplyAssigment_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var mockResponse = new OperationResult<List<SupplyAssigmentResponse>> { IsError = false, Payload = new List<SupplyAssigmentResponse>() };
        _supplyAssigmentServiceMock.Setup(x => x.GetSupplyAssigment()).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplyAssigmentController.GetSupplyAssigment();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetSupplyAssigment_ReturnsNotFound_WhenServiceFails()
    {
        // Arrange
        var mockResponse = new OperationResult<List<SupplyAssigmentResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No supply assignments found." } } };
        _supplyAssigmentServiceMock.Setup(x => x.GetSupplyAssigment()).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplyAssigmentController.GetSupplyAssigment();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task CreateSupplyAssigment_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var requestModel = new CreateSupplyAssigment();
        var mockResponse = new OperationResult<SupplyAssigmentResponse> { IsError = false, Payload = new SupplyAssigmentResponse() };
        _supplyAssigmentServiceMock.Setup(x => x.CreateSupplyAssigment(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplyAssigmentController.CreateSupplyAssigment(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task CreateSupplyAssigment_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var requestModel = new CreateSupplyAssigment();
        var mockResponse = new OperationResult<SupplyAssigmentResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid request." } } };
        _supplyAssigmentServiceMock.Setup(x => x.CreateSupplyAssigment(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplyAssigmentController.CreateSupplyAssigment(requestModel);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Remove_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
        _supplyAssigmentServiceMock.Setup(x => x.RemoveSupplyAssigment(supplierId, partnerId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplyAssigmentController.Remove(supplierId, partnerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task Remove_ReturnsNotFound_WhenServiceFails()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Relationship not found." } } };
        _supplyAssigmentServiceMock.Setup(x => x.RemoveSupplyAssigment(supplierId, partnerId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplyAssigmentController.Remove(supplierId, partnerId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
}