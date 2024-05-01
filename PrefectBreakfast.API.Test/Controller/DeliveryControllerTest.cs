using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class DeliveryControllerTest : SetupTest
{
    private readonly DeliveryController _deliveryController;

    public DeliveryControllerTest()
    {
        _deliveryController = new DeliveryController(_deliveryServiceMock.Object);
    }
    
    [Fact]
    public async Task GetDeliveries_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<DeliveryResponseModel>>
        {
            Payload = new List<DeliveryResponseModel>(),
            IsError = false
        };
        _deliveryServiceMock.Setup(x => x.GetDeliveries()).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.GetDeliveries();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDeliveries_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<DeliveryResponseModel>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error fetching deliveries" } }
        };
        _deliveryServiceMock.Setup(x => x.GetDeliveries()).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.GetDeliveries();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateDelivery_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var requestModel = new CreateDeliveryRequest("", "", "", 1 ,1 ,1);
        var mockResponse = new OperationResult<DeliveryResponseModel>
        {
            Payload = new DeliveryResponseModel(Guid.NewGuid(), "", "", "", 1, 1, 1, new List<string>(), new List<string>(), 1),
            IsError = false
        };
        _deliveryServiceMock.Setup(x => x.CreateDelivery(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.CreateDelivery(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateDelivery_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var requestModel = new CreateDeliveryRequest("", "", "", 1 ,1 ,1);
        var mockResponse = new OperationResult<DeliveryResponseModel>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid request" } }
        };
        _deliveryServiceMock.Setup(x => x.CreateDelivery(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.CreateDelivery(requestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateDelivery_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var requestModel = new UpdateDeliveryRequest();
        var mockResponse = new OperationResult<DeliveryResponseModel>
        {
            Payload = new DeliveryResponseModel(Guid.NewGuid(), "", "", "", 1, 1, 1, new List<string>(), new List<string>(), 1),
            IsError = false
        };
        _deliveryServiceMock.Setup(x => x.UpdateDelivery(id, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.UpdateDelivery(id, requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateDelivery_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var requestModel = new UpdateDeliveryRequest();
        var mockResponse = new OperationResult<DeliveryResponseModel>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Delivery not found" } }
        };
        _deliveryServiceMock.Setup(x => x.UpdateDelivery(id, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.UpdateDelivery(id, requestModel);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task RemoveDelivery_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<DeliveryResponseModel>
        {
            Payload = new DeliveryResponseModel(Guid.NewGuid(), "", "", "", 1, 1, 1, new List<string>(), new List<string>(), 1),
            IsError = false
        };
        _deliveryServiceMock.Setup(x => x.RemoveDelivery(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.RemoveDelivery(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task RemoveDelivery_ReturnsNotFound_WhenDeliveryNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<DeliveryResponseModel>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Delivery not found" } }
        };
        _deliveryServiceMock.Setup(x => x.RemoveDelivery(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.RemoveDelivery(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result); // Corrected to check for NotFoundObjectResult
    }

    
    [Fact]
    public async Task GetDeliveryId_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<DeliveryDetailResponse>
        {
            Payload = new DeliveryDetailResponse(Guid.NewGuid(), "", "", "", 1, 1, 1, new List<string>(), new List<CompanyDto>(), 1),
            IsError = false
        };
        _deliveryServiceMock.Setup(x => x.GetDeliveryId(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.GetDeliveryId(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDeliveryId_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<DeliveryDetailResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Delivery not found" } }
        };
        _deliveryServiceMock.Setup(x => x.GetDeliveryId(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.GetDeliveryId(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetDeliveryPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var searchTerm = "test";
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DeliveryResponseModel>>
        {
            Payload = new Pagination<DeliveryResponseModel>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 20,
                Items = new List<DeliveryResponseModel>()
            },
            IsError = false
        };
        _deliveryServiceMock.Setup(x => x.GetDeliveryUnitPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.GetDeliveryPagination(searchTerm, pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDeliveryPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var searchTerm = "test";
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DeliveryResponseModel>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error fetching pagination data" } }
        };
        _deliveryServiceMock.Setup(x => x.GetDeliveryUnitPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _deliveryController.GetDeliveryPagination(searchTerm, pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    
}