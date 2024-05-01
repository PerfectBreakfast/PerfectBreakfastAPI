using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.MealModels.Request;
using PerfectBreakfast.Application.Models.MealModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class MealControllerTests : SetupTest
{
    private readonly MealController _mealController;

    public MealControllerTests()
    {
        _mealController = new MealController(_mealServiceMock.Object);
    }
    
    [Fact]
    public async Task GetMeals_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<MealResponse>>
        {
            Payload = new List<MealResponse>(),
            IsError = false
        };
        _mealServiceMock.Setup(x => x.GetMeals()).ReturnsAsync(mockResponse);

        // Act
        var result = await _mealController.GetMeals();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetMeals_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<MealResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _mealServiceMock.Setup(x => x.GetMeals()).ReturnsAsync(mockResponse);

        // Act
        var result = await _mealController.GetMeals();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateMeal_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var request = new CreateMealRequest("mama");  // Assume necessary details are filled
        var mockResponse = new OperationResult<MealResponse>
        {
            Payload = new MealResponse(Guid.NewGuid(), "mama",null,null),
            IsError = false
        };
        _mealServiceMock.Setup(x => x.CreateMeal(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _mealController.CreateMeal(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateMeal_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var request = new CreateMealRequest("mama");
        var mockResponse = new OperationResult<MealResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid request data" } }
        };
        _mealServiceMock.Setup(x => x.CreateMeal(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _mealController.CreateMeal(request);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetMealByWorker_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<MealSubscriptionResponse>>
        {
            Payload = new List<MealSubscriptionResponse>(),
            IsError = false
        };
        _mealServiceMock.Setup(x => x.GetMealByWorker()).ReturnsAsync(mockResponse);

        // Act
        var result = await _mealController.GetMealByWorker();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetMealByWorker_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<MealSubscriptionResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error fetching meals for worker" } }
        };
        _mealServiceMock.Setup(x => x.GetMealByWorker()).ReturnsAsync(mockResponse);

        // Act
        var result = await _mealController.GetMealByWorker();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
}