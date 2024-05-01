using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class CategoryControllerTests : SetupTest
{
    private readonly CategoryController _categoryController;

    public CategoryControllerTests()
    {
        _categoryController = new CategoryController(_categoryServiceMock.Object);
    }
    
    // Test for GetAllCategory
    [Fact]
    public async Task GetAllCategory_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<CategoryResponse>>
        {
            Payload = new List<CategoryResponse>(),  // Assume CategoryResponse is correctly populated for the test
            IsError = false
        };
        _categoryServiceMock.Setup(x => x.GetAllCategorys()).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.GetAllCategory();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetAllCategory_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<CategoryResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _categoryServiceMock.Setup(x => x.GetAllCategorys()).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.GetAllCategory();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    // Test for GetCategoryById
    [Fact]
    public async Task GetCategoryById_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var status = FoodStatus.Retail;
        var mockResponse = new OperationResult<CategoryDetailFood>
        {
            Payload = new CategoryDetailFood { Id = categoryId, Name = "", Image = ""},  // Assume detailed response
            IsError = false
        };
        _categoryServiceMock.Setup(x => x.GetCategoryId(categoryId, status)).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.GetCategoryById(categoryId, status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCategoryById_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var status = FoodStatus.Retail;
        var mockResponse = new OperationResult<CategoryDetailFood>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Category not found" } }
        };
        _categoryServiceMock.Setup(x => x.GetCategoryId(categoryId, status)).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.GetCategoryById(categoryId, status);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    // Test for CreateCategory
    [Fact]
    public async Task CreateCategory_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var requestModel = new CreateCategoryRequest();
        var mockResponse = new OperationResult<CategoryResponse>
        {
            Payload = new CategoryResponse(),  // Assume response is filled properly
            IsError = false
        };
        _categoryServiceMock.Setup(x => x.CreateCategory(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.CreateCategory(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateCategory_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var requestModel = new CreateCategoryRequest{Name = "adsasdas"};
        var mockResponse = new OperationResult<CategoryResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid data" } }
        };
        _categoryServiceMock.Setup(x => x.CreateCategory(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.CreateCategory(requestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    // Test for UpdateCategory
    [Fact]
    public async Task UpdateCategory_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var requestModel = new UpdateCategoryRequest();
        var mockResponse = new OperationResult<CategoryResponse>
        {
            Payload = new CategoryResponse(),  // Assume proper filling
            IsError = false
        };
        _categoryServiceMock.Setup(x => x.UpdateCategory(categoryId, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.UpdateCategory(categoryId, requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var requestModel = new UpdateCategoryRequest();
        var mockResponse = new OperationResult<CategoryResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Category not found" } }
        };
        _categoryServiceMock.Setup(x => x.UpdateCategory(categoryId, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.UpdateCategory(categoryId, requestModel);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    // Test for RemoveCategory
    [Fact]
    public async Task RemoveCategory_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var mockResponse = new OperationResult<CategoryResponse>
        {
            Payload = new CategoryResponse(),  // Assume some details about the removal
            IsError = false
        };
        _categoryServiceMock.Setup(x => x.RemoveCategory(categoryId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.RemoveCategory(categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task RemoveCategory_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var mockResponse = new OperationResult<CategoryResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Category not found" } }
        };
        _categoryServiceMock.Setup(x => x.RemoveCategory(categoryId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _categoryController.RemoveCategory(categoryId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}