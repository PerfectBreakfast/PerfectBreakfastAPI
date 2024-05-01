using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class SupplierControllerTest : SetupTest
{
    private readonly SupplierController _supplierController;

    public SupplierControllerTest()
    {
        _supplierController = new SupplierController(_supplierServiceMock.Object);
    }
    
    [Fact]
    public async Task GetSuppliers_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<SupplierResponse>> { Payload = new List<SupplierResponse>(), IsError = false };
        _supplierServiceMock.Setup(x => x.GetSuppliers()).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.GetSuppliers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetSuppliers_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<SupplierResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to fetch suppliers." } } };
        _supplierServiceMock.Setup(x => x.GetSuppliers()).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.GetSuppliers();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task GetPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<Pagination<SupplierResponse>> { Payload = new Pagination<SupplierResponse>(), IsError = false };
        _supplierServiceMock.Setup(x => x.GetPaginationAsync(null, 0, 10)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.GetPagination(null, 0, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<Pagination<SupplierResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to fetch paginated suppliers." } } };
        _supplierServiceMock.Setup(x => x.GetPaginationAsync(null, 0, 10)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.GetPagination(null, 0, 10);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetSupplierId_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<SupplierDetailResponse> { Payload = new SupplierDetailResponse(), IsError = false };
        _supplierServiceMock.Setup(x => x.GetSupplierId(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.GetSupplierId(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetSupplierId_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<SupplierDetailResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to fetch supplier details." } } };
        _supplierServiceMock.Setup(x => x.GetSupplierId(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.GetSupplierId(id);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task CreateSupplier_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var requestModel = new CreateSupplierRequestModel();
        var mockResponse = new OperationResult<SupplierResponse> { Payload = new SupplierResponse(Guid.NewGuid(), "", "", "", 1, 1, new List<string>(),new List<string?>(), 1), IsError = false };
        _supplierServiceMock.Setup(x => x.CreateSupplier(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.CreateSupplier(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateSupplier_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var requestModel = new CreateSupplierRequestModel();
        var mockResponse = new OperationResult<SupplierResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid supplier data." } } };
        _supplierServiceMock.Setup(x => x.CreateSupplier(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.CreateSupplier(requestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
   
    [Fact]
    public async Task RemoveSupplier_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<SupplierResponse> { Payload = new SupplierResponse(Guid.NewGuid(), "", "", "", 1, 1, new List<string>(),new List<string?>(), 1), IsError = false };
        _supplierServiceMock.Setup(x => x.RemoveSupplier(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.RemoveSupplier(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task RemoveSupplier_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<SupplierResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to remove supplier." } } };
        _supplierServiceMock.Setup(x => x.RemoveSupplier(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierController.RemoveSupplier(id);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
public async Task UpdateSupplier_ReturnsOk_WhenNoErrors()
{
    // Arrange
    var id = Guid.NewGuid();
    var requestModel = new UpdateSupplierRequestModel();
    var mockResponse = new OperationResult<SupplierResponse> { Payload = new SupplierResponse(Guid.NewGuid(), "", "", "", 1, 1, new List<string>(),new List<string?>(), 1), IsError = false };
    _supplierServiceMock.Setup(x => x.UpdateSupplier(id, requestModel)).ReturnsAsync(mockResponse);

    // Act
    var result = await _supplierController.UpdateSupplier(id, requestModel);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal(mockResponse.Payload, okResult.Value);
}

[Fact]
public async Task UpdateSupplier_ReturnsError_WhenServiceErrors()
{
    // Arrange
    var id = Guid.NewGuid();
    var requestModel = new UpdateSupplierRequestModel();
    var mockResponse = new OperationResult<SupplierResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to update supplier." } } };
    _supplierServiceMock.Setup(x => x.UpdateSupplier(id, requestModel)).ReturnsAsync(mockResponse);

    // Act
    var result = await _supplierController.UpdateSupplier(id, requestModel);

    // Assert
    Assert.IsType<ObjectResult>(result);
}

[Fact]
public async Task GetSupplierByPartner_ReturnsOk_WhenNoErrors()
{
    // Arrange
    var searchTerm = "test";
    var pageIndex = 1;
    var pageSize = 10;
    var mockResponse = new OperationResult<Pagination<SupplierDTO>> { Payload = new Pagination<SupplierDTO>(), IsError = false };
    _supplierServiceMock.Setup(x => x.GetSupplierByPartner(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

    // Act
    var result = await _supplierController.GetSupplierByPartner(searchTerm, pageIndex, pageSize);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal(mockResponse.Payload, okResult.Value);
}

[Fact]
public async Task GetSupplierByPartner_ReturnsError_WhenServiceErrors()
{
    // Arrange
    var searchTerm = "test";
    var pageIndex = 1;
    var pageSize = 10;
    var mockResponse = new OperationResult<Pagination<SupplierDTO>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to fetch suppliers by partner." } } };
    _supplierServiceMock.Setup(x => x.GetSupplierByPartner(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

    // Act
    var result = await _supplierController.GetSupplierByPartner(searchTerm, pageIndex, pageSize);

    // Assert
    Assert.IsType<ObjectResult>(result);
}

[Fact]
public async Task GetAllSupplierByPartner_ReturnsOk_WhenNoErrors()
{
    // Arrange
    var mockResponse = new OperationResult<List<SupplierDTO>> { Payload = new List<SupplierDTO>(), IsError = false };
    _supplierServiceMock.Setup(x => x.GetAllSupplierByPartner()).ReturnsAsync(mockResponse);

    // Act
    var result = await _supplierController.GetSupplierByPartner();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal(mockResponse.Payload, okResult.Value);
}

[Fact]
public async Task GetAllSupplierByPartner_ReturnsError_WhenServiceErrors()
{
    // Arrange
    var mockResponse = new OperationResult<List<SupplierDTO>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to fetch all suppliers by partner." } } };
    _supplierServiceMock.Setup(x => x.GetAllSupplierByPartner()).ReturnsAsync(mockResponse);

    // Act
    var result = await _supplierController.GetSupplierByPartner();

    // Assert
    Assert.IsType<ObjectResult>(result);
}

[Fact]
public async Task GetSupplierByFood_ReturnsOk_WhenNoErrors()
{
    // Arrange
    var id = Guid.NewGuid();
    var mockResponse = new OperationResult<List<SupplierDTO>> { Payload = new List<SupplierDTO>(), IsError = false };
    _supplierServiceMock.Setup(x => x.GetSupplierByFood(id)).ReturnsAsync(mockResponse);

    // Act
    var result = await _supplierController.GetSupplierByFood(id);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal(mockResponse.Payload, okResult.Value);
}

[Fact]
public async Task GetSupplierByFood_ReturnsError_WhenServiceErrors()
{
    // Arrange
    var id = Guid.NewGuid();
    var mockResponse = new OperationResult<List<SupplierDTO>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to fetch suppliers by food." } } };
    _supplierServiceMock.Setup(x => x.GetSupplierByFood(id)).ReturnsAsync(mockResponse);

    // Act
    var result = await _supplierController.GetSupplierByFood(id);

    // Assert
    Assert.IsType<ObjectResult>(result);
}

}