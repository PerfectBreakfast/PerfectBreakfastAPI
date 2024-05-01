using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CompanyModels.Request;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class CompanyControllerTests : SetupTest
{
    private readonly CompanyController _companyController;

    public CompanyControllerTests()
    {
        _companyController = new CompanyController(_companyServiceMock.Object);
    }
    
    [Fact]
    public async Task GetCompanies_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<CompanyResponse>>
        {
            Payload = new List<CompanyResponse>(),
            IsError = false
        };
        _companyServiceMock.Setup(x => x.GetAllCompanies()).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCompanies_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<CompanyResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _companyServiceMock.Setup(x => x.GetAllCompanies()).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompanies();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    // Additional Tests for other methods

    [Fact]
    public async Task GetCompany_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockResponse = new OperationResult<CompanyResponse>
        {
            Payload = new CompanyResponse(),
            IsError = false
        };
        _companyServiceMock.Setup(x => x.GetCompany(companyId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompany(companyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCompany_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockResponse = new OperationResult<CompanyResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Company not found" } }
        };
        _companyServiceMock.Setup(x => x.GetCompany(companyId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompany(companyId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateCompany_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var companyRequest = new CompanyRequest(); // Assume details filled
        var mockResponse = new OperationResult<bool>
        {
            Payload = true,
            IsError = false
        };
        _companyServiceMock.Setup(x => x.CreateCompany(companyRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.CreateCompany(companyRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateCompany_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var companyRequest = new CompanyRequest();
        var mockResponse = new OperationResult<bool>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid data" } }
        };
        _companyServiceMock.Setup(x => x.CreateCompany(companyRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.CreateCompany(companyRequest);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateCompany_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var companyRequest = new UpdateCompanyRequest(); // Assume details filled
        var mockResponse = new OperationResult<bool>
        {
            Payload = true,
            IsError = false
        };
        _companyServiceMock.Setup(x => x.UpdateCompany(companyId, companyRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.UpdateCompany(companyId, companyRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateCompany_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var companyRequest = new UpdateCompanyRequest();
        var mockResponse = new OperationResult<bool>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Company not found" } }
        };
        _companyServiceMock.Setup(x => x.UpdateCompany(companyId, companyRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.UpdateCompany(companyId, companyRequest);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteCompany_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockResponse = new OperationResult<CompanyResponse>
        {
            Payload = new CompanyResponse(),
            IsError = false
        };
        _companyServiceMock.Setup(x => x.DeleteCompany(companyId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.Delete(companyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task DeleteCompany_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockResponse = new OperationResult<CompanyResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Company not found" } }
        };
        _companyServiceMock.Setup(x => x.DeleteCompany(companyId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.Delete(companyId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetUsersByCompany_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<UserModelResponse>>
        {
            Payload = new List<UserModelResponse>(),
            IsError = false
        };
        _companyServiceMock.Setup(x => x.GetUsersByCompanyId(companyId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetUsersByCompany(companyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetUsersByCompany_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<UserModelResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving users" } }
        };
        _companyServiceMock.Setup(x => x.GetUsersByCompanyId(companyId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetUsersByCompany(companyId);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetCompanyPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var searchTerm = "test";
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<CompanyResponsePaging>>
        {
            Payload = new Pagination<CompanyResponsePaging>
            {
                Items = new List<CompanyResponsePaging>(), // Assume the details are filled
                PageIndex = pageIndex,
                PageSize = pageSize,
            },
            IsError = false
        };
        _companyServiceMock.Setup(x => x.GetCompanyPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompanyPagination(searchTerm, pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCompanyPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var searchTerm = "test";
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<CompanyResponsePaging>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error during pagination" } }
        };
        _companyServiceMock.Setup(x => x.GetCompanyPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompanyPagination(searchTerm, pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetCompaniesByPartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var searchTerm = "partner";
        var pageIndex = 1;
        var pageSize = 5;
        var mockResponse = new OperationResult<Pagination<CompanyResponsePaging>>
        {
            Payload = new Pagination<CompanyResponsePaging>
            {
                TotalItemsCount = 10,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = new List<CompanyResponsePaging>()
            },
            IsError = false
        };
        _companyServiceMock.Setup(x => x.GetCompanyByPartner(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompaniesByPartner(searchTerm, pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCompaniesByPartner_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var searchTerm = "partner";
        var pageIndex = 1;
        var pageSize = 5;
        var mockResponse = new OperationResult<Pagination<CompanyResponsePaging>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving companies" } }
        };
        _companyServiceMock.Setup(x => x.GetCompanyByPartner(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompaniesByPartner(searchTerm, pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetCompaniesByDelivery_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var searchTerm = "delivery";
        var pageIndex = 2;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<CompanyResponsePaging>>
        {
            Payload = new Pagination<CompanyResponsePaging>
            {
                TotalItemsCount = 20,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = new List<CompanyResponsePaging>()
            },
            IsError = false
        };
        _companyServiceMock.Setup(x => x.GetCompanyByDelivery(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompaniesByDelivery(searchTerm, pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCompaniesByDelivery_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var searchTerm = "delivery";
        var pageIndex = 2;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<CompanyResponsePaging>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error during pagination" } }
        };
        _companyServiceMock.Setup(x => x.GetCompanyByDelivery(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.GetCompaniesByDelivery(searchTerm, pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task SearchCompanies_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var searchTerm = "search";
        var mockResponse = new OperationResult<List<CompanyResponse>>
        {
            Payload = new List<CompanyResponse>(),
            IsError = false
        };
        _companyServiceMock.Setup(x => x.SearchCompany(searchTerm)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.SearchCompanies(searchTerm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task SearchCompanies_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var searchTerm = "search";
        var mockResponse = new OperationResult<List<CompanyResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error searching companies" } }
        };
        _companyServiceMock.Setup(x => x.SearchCompany(searchTerm)).ReturnsAsync(mockResponse);

        // Act
        var result = await _companyController.SearchCompanies(searchTerm);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
}