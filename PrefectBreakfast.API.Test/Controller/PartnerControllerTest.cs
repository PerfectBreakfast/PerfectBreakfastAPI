using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.PartnerModels.Request;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class PartnerControllerTest : SetupTest
{
    private readonly PartnerController _partnerController;

    public PartnerControllerTest()
    {
        _partnerController = new PartnerController(_partnerServiceMock.Object);
    }
    
    [Fact]
    public async Task GetPartners_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<PartnerResponseModel>>
        {
            Payload = new List<PartnerResponseModel>
            {
                new PartnerResponseModel (Guid.NewGuid(),  "", "", "", 1, 1, 1, new List<string>(), new List<string?>(), new List<string?>(), 2),
                new PartnerResponseModel (Guid.NewGuid(),  "", "", "", 1, 1, 1, new List<string>(), new List<string?>(), new List<string?>(), 2)
            },
            IsError = false
        };
        _partnerServiceMock.Setup(x => x.GetPartners()).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.GetPartners();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetPartners_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<PartnerResponseModel>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No partners found." } }
        };
        _partnerServiceMock.Setup(x => x.GetPartners()).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.GetPartners();

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task CreatePartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var requestModel = new CreatePartnerRequest();
        var mockResponse = new OperationResult<PartnerResponseModel>
        {
            Payload = new PartnerResponseModel(Guid.NewGuid(),  "", "", "", 1, 1, 1, new List<string>(), new List<string?>(), new List<string?>(), 2),
            IsError = false
        };
        _partnerServiceMock.Setup(x => x.CreatePartner(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.CreatePartner(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreatePartner_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var requestModel = new CreatePartnerRequest();
        var mockResponse = new OperationResult<PartnerResponseModel>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error creating partner." } }
        };
        _partnerServiceMock.Setup(x => x.CreatePartner(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.CreatePartner(requestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdatePartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        UpdatePartnerRequest requestModel = new UpdatePartnerRequest();
        var mockResponse = new OperationResult<PartnerResponseModel>
        {
            Payload = new PartnerResponseModel(Guid.NewGuid(),  "", "", "", 1, 1, 1, new List<string>(), new List<string?>(), new List<string?>(), 2),
            IsError = false
        };
        _partnerServiceMock.Setup(x => x.UpdatePartner(id, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.UpdatePartner(id, requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdatePartner_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        UpdatePartnerRequest requestModel = new UpdatePartnerRequest ();
        var mockResponse = new OperationResult<PartnerResponseModel>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Partner not found." } }
        };
        _partnerServiceMock.Setup(x => x.UpdatePartner(id, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.UpdatePartner(id, requestModel);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task RemovePartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var mockResponse = new OperationResult<PartnerResponseModel>
        {
            Payload = new PartnerResponseModel(Guid.NewGuid(),  "", "", "", 1, 1, 1, new List<string>(), new List<string?>(), new List<string?>(), 2),
            IsError = false
        };
        _partnerServiceMock.Setup(x => x.RemovePartner(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.RemovePartner(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task RemovePartner_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var mockResponse = new OperationResult<PartnerResponseModel>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Partner not found." } }
        };
        _partnerServiceMock.Setup(x => x.RemovePartner(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.RemovePartner(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetPartnerId_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var mockResponse = new OperationResult<PartnerDetailResponse>
        {
            Payload = new PartnerDetailResponse (Guid.NewGuid(),  "", "", "", 1, 1, 1, new List<SupplierDTO?>(), new List<CompanyResponsePaging>()),
            IsError = false
        };
        _partnerServiceMock.Setup(x => x.GetPartnerById(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.GetPartnerId(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetPartnerId_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var mockResponse = new OperationResult<PartnerDetailResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Partner not found." } }
        };
        _partnerServiceMock.Setup(x => x.GetPartnerById(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.GetPartnerId(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetPartnerPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        string searchTerm = "search";
        int pageIndex = 1;
        int pageSize = 10;
        var mockResponse = new OperationResult<Pagination<PartnerResponseModel>>
        {
            Payload = new Pagination<PartnerResponseModel>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 20,
                Items = new List<PartnerResponseModel> 
                    { new PartnerResponseModel (Guid.NewGuid(),  "", "", "", 1, 1, 1, new List<string>(), new List<string?>(), new List<string?>(), 2) }
            },
            IsError = false
        };
        _partnerServiceMock.Setup(x => x.GetPartnerPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.GetPartnerPagination(searchTerm, pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetPartnerPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        string searchTerm = "search";
        int pageIndex = 1;
        int pageSize = 10;
        var mockResponse = new OperationResult<Pagination<PartnerResponseModel>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error fetching pagination." } }
        };
        _partnerServiceMock.Setup(x => x.GetPartnerPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.GetPartnerPagination(searchTerm, pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task AssignPartnerToSupplier_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid supplierId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<PartnerResponseModel>>
        {
            Payload = new List<PartnerResponseModel> { new PartnerResponseModel(Guid.NewGuid(),  "", "", "", 1, 1, 1, new List<string>(), new List<string?>(), new List<string?>(), 2) },
            IsError = false
        };
        _partnerServiceMock.Setup(x => x.AssignPartnerToSupplier(supplierId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.GetPartnerToSupplierAssign(supplierId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task AssignPartnerToSupplier_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid supplierId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<PartnerResponseModel>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error assigning partner to supplier." } }
        };
        _partnerServiceMock.Setup(x => x.AssignPartnerToSupplier(supplierId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _partnerController.GetPartnerToSupplierAssign(supplierId);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

}