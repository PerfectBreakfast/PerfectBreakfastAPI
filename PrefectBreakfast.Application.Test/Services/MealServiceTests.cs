using FluentAssertions;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MealModels.Request;
using PerfectBreakfast.Application.Models.MealModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class MealServiceTests : SetupTest
{
    private readonly IMealService _mealService;

    public MealServiceTests()
    {
        _mealService = new MealService(_unitOfWorkMock.Object, _mapper, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetMeals_ReturnsSuccess_WithEmptyList()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.MealRepository.GetAllAsync())
            .ReturnsAsync(new List<Meal>());

        // Act
        var result = await _mealService.GetMeals();

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Count.Should().Be(0);
    }
    
    [Fact]
    public async Task GetMeals_ReturnsSuccess_WithPopulatedList()
    {
        // Arrange
        var mockMeals = new List<Meal>()
        {
            new Meal { Id = Guid.NewGuid(), MealType = "Meal 1" },
            new Meal { Id = Guid.NewGuid(), MealType = "Meal 2" }
        };

        _unitOfWorkMock.Setup(u => u.MealRepository.GetAllAsync())
            .ReturnsAsync(mockMeals);

        /*_mapperMock.Setup(m => m.Map<List<MealResponse>>(It.IsAny<List<Meal>>()))
            .Returns(new List<MealResponse>() { new MealResponse(), new MealResponse() });*/

        // Act
        var result = await _mealService.GetMeals();

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Count.Should().Be(2);
    }
    
    [Fact]
    public async Task GetMeals_ReturnsError_OnException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.MealRepository.GetAllAsync())
            .ThrowsAsync(new Exception("Error retrieving meals"));

        // Act
        var result = await _mealService.GetMeals();

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Error retrieving meals"));
    }
    
    [Fact]
    public async Task GetMealByWorker_ReturnsSuccess_WithSubscriptions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockUser = new User
        {
            Id = userId, 
            Company = new Company
            {
                Address = "",
                Email = "",
                Name = "",
                PhoneNumber = "",
                MealSubscriptions = new List<MealSubscription?>
                {
                    new MealSubscription
                    {
                        Id = Guid.NewGuid(),
                        Meal = new Meal
                        {
                            Id = Guid.NewGuid(),
                            MealType = "hehe"
                        }
                    }
                }
            }
        };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(mockUser);

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);

        // Act
        var result = await _mealService.GetMealByWorker();

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task GetMealByWorker_ReturnsError_OnException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<IncludeInfo<User>>()))
            .ThrowsAsync(new Exception("Error"));
        
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(Guid.NewGuid);

        // Act
        var result = await _mealService.GetMealByWorker();

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Error"));
    }
    
    [Fact]
    public async Task CreateMeal_ReturnsSuccess_WithValidRequest()
    {
        // Arrange
        var mockRequest = new CreateMealRequest("Test Meal");

        var mockMeal = new Meal { Id = Guid.NewGuid(), MealType = mockRequest.MealType };

        _unitOfWorkMock.Setup(u => u.MealRepository.AddAsync(It.IsAny<Meal>()))
            .ReturnsAsync(mockMeal);

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ReturnsAsync(1);

        /*_mapperMock.Setup(m => m.Map<Meal>(mockRequest))
            .Returns(mockMeal);

        _mapperMock.Setup(m => m.Map<MealResponse>(mockMeal))
            .Returns(new MealResponse());*/

        // Act
        var result = await _mealService.CreateMeal(mockRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.MealType.Should().Be(mockRequest.MealType);
    }
    
    [Fact]
    public async Task CreateMeal_ReturnsError_OnException()
    {
        // Arrange
        var mockRequest = new CreateMealRequest("Test Meal");

        _unitOfWorkMock.Setup(u => u.MealRepository.AddAsync(It.IsAny<Meal>()))
            .ThrowsAsync(new Exception("Error saving meal"));

        /*_mapperMock.Setup(m => m.Map<Meal>(mockRequest))
            .Returns(new Meal());*/

        // Act
        var result = await _mealService.CreateMeal(mockRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Error saving meal"));
    }
}