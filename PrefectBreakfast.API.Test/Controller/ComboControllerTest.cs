using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ComboModels.Request;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class ComboControllerTest : SetupTest
{
    private readonly ComboController _comboController;

    public ComboControllerTest()
    {
        _comboController = new ComboController(_comboServiceMock.Object);
    }
    
    [Fact]
    public async Task GetCombos_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<ComboResponse>>
        {
            Payload = new List<ComboResponse>(),
            IsError = false
        };
        _comboServiceMock.Setup(x => x.GetCombos()).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.GetCombos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCombos_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<ComboResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _comboServiceMock.Setup(x => x.GetCombos()).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.GetCombos();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetCombo_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var mockResponse = new OperationResult<ComboDetailResponse>
        {
            Payload = new ComboDetailResponse(),
            IsError = false
        };
        _comboServiceMock.Setup(x => x.GetCombo(comboId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.GetCombo(comboId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCombo_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var mockResponse = new OperationResult<ComboDetailResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Combo not found" } }
        };
        _comboServiceMock.Setup(x => x.GetCombo(comboId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.GetCombo(comboId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateCombo_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var createComboRequest = new CreateComboRequest();  // Assume necessary details are filled
        var mockResponse = new OperationResult<ComboResponse>
        {
            Payload = new ComboResponse(),
            IsError = false
        };
        _comboServiceMock.Setup(x => x.CreateCombo(createComboRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.CreateCombo(createComboRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateCombo_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var createComboRequest = new CreateComboRequest();
        var mockResponse = new OperationResult<ComboResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid data" } }
        };
        _comboServiceMock.Setup(x => x.CreateCombo(createComboRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.CreateCombo(createComboRequest);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateCombo_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var updateComboRequest = new UpdateComboRequest();  // Assume necessary details are filled
        var mockResponse = new OperationResult<ComboResponse>
        {
            Payload = new ComboResponse(),
            IsError = false
        };
        _comboServiceMock.Setup(x => x.UpdateCombo(comboId, updateComboRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.UpdateCombo(comboId, updateComboRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateCombo_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var updateComboRequest = new UpdateComboRequest();
        var mockResponse = new OperationResult<ComboResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Combo not found" } }
        };
        _comboServiceMock.Setup(x => x.UpdateCombo(comboId, updateComboRequest)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.UpdateCombo(comboId, updateComboRequest);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    
    [Fact]
    public async Task GetComboPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var searchTerm = "test";
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<ComboResponse>>
        {
            Payload = new Pagination<ComboResponse>
            {
                TotalItemsCount = 100,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = new List<ComboResponse>()
            },
            IsError = false
        };
        _comboServiceMock.Setup(x => x.GetComboPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.GetComboPagination(searchTerm, pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetComboPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var searchTerm = "test";
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<ComboResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error during pagination" } }
        };
        _comboServiceMock.Setup(x => x.GetComboPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.GetComboPagination(searchTerm, pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task Delete_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var mockResponse = new OperationResult<ComboResponse>
        {
            Payload = new ComboResponse(),
            IsError = false
        };
        _comboServiceMock.Setup(x => x.DeleteCombo(comboId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.Delete(comboId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var mockResponse = new OperationResult<ComboResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Combo not found" } }
        };
        _comboServiceMock.Setup(x => x.DeleteCombo(comboId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _comboController.Delete(comboId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

}