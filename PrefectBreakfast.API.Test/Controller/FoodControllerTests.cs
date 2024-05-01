using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class FoodControllerTests : SetupTest
{
    private readonly FoodController _foodController;

    public FoodControllerTests()
    {
        _foodController = new FoodController(_foodServiceMock.Object);
    }
    
    [Fact]
    public async Task GetAllFood_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<FoodResponse>>
        {
            Payload = new List<FoodResponse>(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.GetAllFoods()).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetAllFood();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetAllFood_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<FoodResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _foodServiceMock.Setup(x => x.GetAllFoods()).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetAllFood();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetFoodByFoodStatus_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var status = FoodStatus.Retail;
        var mockResponse = new OperationResult<List<FoodResponse>>
        {
            Payload = new List<FoodResponse>(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.GetFoodByFoodStatus(status)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodByFoodStatus(status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetFoodByFoodStatus_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var status = FoodStatus.Retail;
        var mockResponse = new OperationResult<List<FoodResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error fetching food by status" } }
        };
        _foodServiceMock.Setup(x => x.GetFoodByFoodStatus(status)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodByFoodStatus(status);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetFoodId_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var mockResponse = new OperationResult<FoodResponseCategory>
        {
            Payload = new FoodResponseCategory(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.GetFoodById(foodId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodId(foodId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetFoodId_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var mockResponse = new OperationResult<FoodResponseCategory>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Food not found" } }
        };
        _foodServiceMock.Setup(x => x.GetFoodById(foodId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodId(foodId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateFood_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var requestModel = new CreateFoodRequestModels();  // Assume details are filled
        var mockResponse = new OperationResult<FoodResponse>
        {
            Payload = new FoodResponse(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.CreateFood(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.CreateFood(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateFood_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var requestModel = new CreateFoodRequestModels();
        var mockResponse = new OperationResult<FoodResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid data" } }
        };
        _foodServiceMock.Setup(x => x.CreateFood(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.CreateFood(requestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateFood_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var requestModel = new UpdateFoodRequestModels();  // Assume details are filled
        var mockResponse = new OperationResult<FoodResponse>
        {
            Payload = new FoodResponse(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.UpdateFood(foodId, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.UpdateFood(foodId, requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateFood_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var requestModel = new UpdateFoodRequestModels();
        var mockResponse = new OperationResult<FoodResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Food not found" } }
        };
        _foodServiceMock.Setup(x => x.UpdateFood(foodId, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.UpdateFood(foodId, requestModel);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task RemoveFood_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var mockResponse = new OperationResult<FoodResponse>
        {
            Payload = new FoodResponse(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.RemoveFood(foodId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.RemoveFood(foodId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task RemoveFood_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var mockResponse = new OperationResult<FoodResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Food not found" } }
        };
        _foodServiceMock.Setup(x => x.RemoveFood(foodId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.RemoveFood(foodId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetFoodPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var searchTerm = "test";
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<FoodResponse>>
        {
            Payload = new Pagination<FoodResponse>
            {
                TotalItemsCount = 20,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = new List<FoodResponse>()
            },
            IsError = false
        };
        _foodServiceMock.Setup(x => x.GetFoodPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodPagination(searchTerm, pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetFoodPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var searchTerm = "test";
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<FoodResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error during pagination" } }
        };
        _foodServiceMock.Setup(x => x.GetFoodPaginationAsync(searchTerm, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodPagination(searchTerm, pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetFoodForPartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<TotalFoodForPartnerResponse>
        {
            Payload = new TotalFoodForPartnerResponse(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.GetComboAndFoodsForPartner(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodForPartner(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetTotalFoodForPartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<TotalFoodForPartnerResponse>
        {
            Payload = new TotalFoodForPartnerResponse(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.GetFoodsForPartner(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetTotalFoodForPartner(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }
    
    [Fact]
    public async Task GetFoodForDelivery_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<TotalFoodForPartnerResponse>
        {
            Payload = new TotalFoodForPartnerResponse(),
            IsError = false
        };
        _foodServiceMock.Setup(x => x.GetComboAndFoodsForDelivery(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodForDelivery(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }
    
    [Fact]
    public async Task GetFoodForSupplier_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var searchTerm = "example";
        var mockResponse = new OperationResult<List<FoodResponse>>
        {
            Payload = new List<FoodResponse>(),  // Assuming FoodResponse is properly defined in your application
            IsError = false
        };
        _foodServiceMock.Setup(x => x.GetFoodForSupplier(supplierId, searchTerm)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodForSupplier(supplierId, searchTerm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetFoodForSupplier_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var searchTerm = "example";
        var mockResponse = new OperationResult<List<FoodResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving food list for supplier" } }
        };
        _foodServiceMock.Setup(x => x.GetFoodForSupplier(supplierId, searchTerm)).ReturnsAsync(mockResponse);

        // Act
        var result = await _foodController.GetFoodForSupplier(supplierId, searchTerm);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);  // Assuming HandleErrorResponse is setting this status code
    }
}