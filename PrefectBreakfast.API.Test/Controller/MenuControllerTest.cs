using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.MenuModels.Request;
using PerfectBreakfast.Application.Models.MenuModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class MenuControllerTest : SetupTest
{
    private readonly MenuController _menuController;

    public MenuControllerTest()
    {
        _menuController = new MenuController(_menuServiceMock.Object);
    }
    
    [Fact]
    public async Task GetMenus_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<MenuResponse>>
        {
            Payload = new List<MenuResponse>(),
            IsError = false
        };
        _menuServiceMock.Setup(x => x.GetMenus()).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.GetMenus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetMenus_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<MenuResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Server error" } }
        };
        _menuServiceMock.Setup(x => x.GetMenus()).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.GetMenus();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateMenu_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var requestModel = new CreateMenuFoodRequest();
        var mockResponse = new OperationResult<MenuResponse>
        {
            Payload = new MenuResponse(),
            IsError = false
        };
        _menuServiceMock.Setup(x => x.CreateMenu(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.CreateMenu(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateMenu_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var requestModel = new CreateMenuFoodRequest();
        var mockResponse = new OperationResult<MenuResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid data" } }
        };
        _menuServiceMock.Setup(x => x.CreateMenu(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.CreateMenu(requestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateMenu_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        var requestModel = new UpdateMenuRequest();
        var mockResponse = new OperationResult<MenuResponse>
        {
            Payload = new MenuResponse(),
            IsError = false
        };
        _menuServiceMock.Setup(x => x.UpdateMenu(id, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.UpdateMenu(id, requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateMenu_ReturnsError_WhenFailed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var requestModel = new UpdateMenuRequest();
        var mockResponse = new OperationResult<MenuResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Menu not found" } }
        };
        _menuServiceMock.Setup(x => x.UpdateMenu(id, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.UpdateMenu(id, requestModel);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetMenu_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<MenuResponse>
        {
            Payload = new MenuResponse(),
            IsError = false
        };
        _menuServiceMock.Setup(x => x.GetMenu(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.GetMenu(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetMenu_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<MenuResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Menu not found" } }
        };
        _menuServiceMock.Setup(x => x.GetMenu(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.GetMenu(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetMenuPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        string searchTerm = "test";
        int pageIndex = 1;
        int pageSize = 10;
        var mockResponse = new OperationResult<Pagination<MenuResponsePaging>>
        {
            Payload = new Pagination<MenuResponsePaging>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 100,
                Items = new List<MenuResponsePaging>()
            },
            IsError = false
        };
        _menuServiceMock.Setup(x => x.GetMenuPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.GetMenuPagination(searchTerm, pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetMenuPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        string searchTerm = "test";
        int pageIndex = 1;
        int pageSize = 10;
        var mockResponse = new OperationResult<Pagination<MenuResponsePaging>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving data" } }
        };
        _menuServiceMock.Setup(x => x.GetMenuPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.GetMenuPagination(searchTerm, pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task Delete_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<MenuResponse>
        {
            Payload = new MenuResponse(),
            IsError = false
        };
        _menuServiceMock.Setup(x => x.DeleteMenu(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.Delete(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<MenuResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Menu not found" } }
        };
        _menuServiceMock.Setup(x => x.DeleteMenu(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.Delete(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task ChooseMenu_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<MenuResponse>
        {
            Payload = new MenuResponse(), // Assuming this response includes details about the menu chosen
            IsError = false
        };
        _menuServiceMock.Setup(x => x.ChooseMenu(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.ChooseMenu(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task ChooseMenu_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<MenuResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Menu not found" } }
        };
        _menuServiceMock.Setup(x => x.ChooseMenu(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.ChooseMenu(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetMenuToShow_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<MenuIsSelectedResponse>
        {
            Payload = new MenuIsSelectedResponse(Guid.NewGuid(), "", true, false, DateTime.Now, new List<ComboAndFoodResponse?>(),new List<FoodResponse?>()), 
            IsError = false
        };
        _menuServiceMock.Setup(x => x.GetMenuByStatus()).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.GetMenuToShow();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetMenuToShow_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<MenuIsSelectedResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No selected menu found" } }
        };
        _menuServiceMock.Setup(x => x.GetMenuByStatus()).ReturnsAsync(mockResponse);

        // Act
        var result = await _menuController.GetMenuToShow();

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    
}