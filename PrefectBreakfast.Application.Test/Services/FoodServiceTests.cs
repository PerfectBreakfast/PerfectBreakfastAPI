using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using MockQueryable.Moq;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class FoodServiceTests : SetupTest
{
    private readonly IFoodService _foodService;

    public FoodServiceTests()
    {
        _foodService = new FoodService(_unitOfWorkMock.Object, _mapper, _imgurServiceMock.Object,
            _claimServiceMock.Object, _currentTimeMock.Object);
    }
    
    [Fact]
    public async Task CreateFood_ImageUploadSuccess()
    {
        // Arrange
        var cate = new Category
        {
            Id = Guid.NewGuid(),
            Name = ""
        };
        const string expectedImageUrl = "http://image.url";
        _imgurServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(expectedImageUrl);
        _unitOfWorkMock.Setup(u => u.FoodRepository.AddAsync(It.IsAny<Food>())).ReturnsAsync(new Food());
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
        //_mockMapper.Setup(m => m.Map<FoodResponse>(It.IsAny<Food>())).Returns(new FoodResponse());

        var requestModel = new CreateFoodRequestModels {
            Name = "Test Food",
            Description = "Description",
            Price = 100,
            Image = null,
            FoodStatus = 0,
            CategoryId = cate.Id
        };

        // Act
        var result = await _foodService.CreateFood(requestModel);

        // Assert
        result.Payload.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CreateFood_ImageUploadFailure()
    {
        // Arrange
        _imgurServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>())).ThrowsAsync(new Exception("Image upload failed"));
        _unitOfWorkMock.Setup(u => u.FoodRepository.AddAsync(It.IsAny<Food>())).ReturnsAsync(new Food());
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
        //_mockMapper.Setup(m => m.Map<FoodResponse>(It.IsAny<Food>())).Returns(new FoodResponse());

        var requestModel = new CreateFoodRequestModels {
            Name = "Test Food",
            Description = "Description",
            Price = 100,
            Image = null,
            FoodStatus = 0
        };

        // Act
        var result = await _foodService.CreateFood(requestModel);

        // Assert
        Assert.Contains("Image upload failed", result.Errors.Select(e => e.Message));
    }
    
    [Fact]
    public async Task CreateFood_ExceptionHandling()
    {
        // Arrange
        _imgurServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>())).ThrowsAsync(new Exception("Unexpected exception"));

        var requestModel = new CreateFoodRequestModels {
            Name = "Test Food",
            Description = "Description",
            Price = 100,
            Image = null,
            FoodStatus = 0
        };

        // Act
        var result = await _foodService.CreateFood(requestModel);

        // Assert
        Assert.Contains("Unexpected exception", result.Errors.Select(e => e.Message));
    }
    
    [Fact]
    public async Task GetAllFoods_Success()
    {
        // Arrange - Create sample data
        var foodEntities = new List<Food> {
            new Food { Id = Guid.NewGuid(), Name = "Food 1" },
            new Food { Id = Guid.NewGuid(), Name = "Food 2" }
        };
        var expectedFoodResponses = new List<FoodResponse> {
            new FoodResponse { Id = foodEntities[0].Id, Name = foodEntities[0].Name },
            new FoodResponse { Id = foodEntities[1].Id, Name = foodEntities[1].Name }
        };

        // Setup mocks
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindAll(It.IsAny<Expression<Func<Food, bool>>>()))
            .Returns(foodEntities.AsQueryable().BuildMock());
        /*_mockMapper.Setup(m => m.Map<List<FoodResponse>>(foodEntities))
            .Returns(expectedFoodResponses);*/

        // Act
        var result = await _foodService.GetAllFoods();

        // Assert
        Assert.Equal(2, result.Payload.Count); // Ensure the correct number
        Assert.Equal(expectedFoodResponses[0].Id, result.Payload[0].Id);
        // ... (add assertions to compare the rest of the properties)
    }
    
    [Fact]
    public async Task GetAllFoods_EmptyResultSet()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindAll(It.IsAny<Expression<Func<Food, bool>>>()))
            .Returns(new List<Food>().AsQueryable().BuildMock);
        /*_mockMapper.Setup(m => m.Map<List<FoodResponse>>(It.IsAny<List<Food>>()))
            .Returns(new List<FoodResponse>());*/

        // Act
        var result = await _foodService.GetAllFoods();

        // Assert
        Assert.Empty(result.Payload);
    }
    
    [Fact]
    public async Task GetAllFoods_DatabaseRetrievalError()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindAll(It.IsAny<Expression<Func<Food, bool>>>()))
            .Throws(new Exception());
        /*_mockMapper.Setup(m => m.Map<List<FoodResponse>>(It.IsAny<List<Food>>()))
            .Returns(new List<FoodResponse>());*/

        // Act
        var result = await _foodService.GetAllFoods();

        // Assert
        result.IsError.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetFoodByFoodStatus_Success_Combo()
    {
        // Arrange
        const FoodStatus status = FoodStatus.Combo; // Assuming FoodStatus is an enum
        var foodEntities = new List<Food> {
            new Food { Id = Guid.NewGuid(), Name = "Food 1", FoodStatus = status },
            new Food { Id = Guid.NewGuid(), Name = "Food 2", FoodStatus = status }
        };
        var expectedFoodResponses = new List<FoodResponse> {
            new FoodResponse { Id = foodEntities[0].Id, Name = foodEntities[0].Name },
            new FoodResponse { Id = foodEntities[1].Id, Name = foodEntities[1].Name }
        };

        // Setup mocks
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindAll(It.IsAny<Expression<Func<Food, bool>>>()))
            .Returns(foodEntities.AsQueryable().BuildMock); // Main Change
        /*_mockMapper.Setup(m => m.Map<List<FoodResponse>>(foodEntities))
            .Returns(expectedFoodResponses);*/

        // Act
        var result = await _foodService.GetFoodByFoodStatus(status);

        // Assert
        result.Payload.Should().NotBeNull();
        // ... rest of the asserts
    }
    
    [Fact]
    public async Task GetFoodByFoodStatus_Success_Retail()
    {
        // Arrange
        const FoodStatus status = FoodStatus.Retail; // Assuming FoodStatus is an enum
        var foodEntities = new List<Food> {
            new Food { Id = Guid.NewGuid(), Name = "Food 3", FoodStatus = status },
            new Food { Id = Guid.NewGuid(), Name = "Food 4", FoodStatus = status }
        };
        var expectedFoodResponses = new List<FoodResponse> {
            new FoodResponse { Id = foodEntities[0].Id, Name = foodEntities[0].Name },
            new FoodResponse { Id = foodEntities[1].Id, Name = foodEntities[1].Name }
        };

        // Setup mocks
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindAll(It.IsAny<Expression<Func<Food, bool>>>()))
            .Returns(foodEntities.AsQueryable().BuildMock);
        /*_mockMapper.Setup(m => m.Map<List<FoodResponse>>(foodEntities))
            .Returns(expectedFoodResponses);*/

        // Act
        var result = await _foodService.GetFoodByFoodStatus(status);

        // Assert
        result.Payload.Should().NotBeNull();
        // ... rest of the asserts
    }
    
    [Fact]
    public async Task GetFoodByFoodStatus_EmptyResultSet()
    {
        // Arrange
        const FoodStatus status = FoodStatus.Combo; // Any FoodStatus value
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindAll(It.IsAny<Expression<Func<Food, bool>>>()))
            .Returns(new List<Food>().AsQueryable().BuildMock);
        /*_mockMapper.Setup(m => m.Map<List<FoodResponse>>(It.IsAny<List<Food>>()))
            .Returns(new List<FoodResponse>());*/

        // Act
        var result = await _foodService.GetFoodByFoodStatus(status);

        // Assert
        Assert.Empty(result.Payload);
    }
    
    [Fact]
    public async Task GetFoodByFoodStatus_DatabaseRetrievalError()
    {
        // Arrange
        const FoodStatus status = FoodStatus.Combo; // Any FoodStatus value
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindAll(It.IsAny<Expression<Func<Food, bool>>>()))
            .Throws(new Exception("Database error"));

        // Act
        var result = await _foodService.GetFoodByFoodStatus(status);

        // Assert
        Assert.Contains("Database error", result.Errors.Select(e => e.Message));
    }
    
    [Fact]
    public async Task GetFoodById_Success()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var expectedCategory = new Category { Id = Guid.NewGuid(), Name = "Category 1" };
        var foodEntity = new Food {
            Id = foodId,
            Name = "Food 1",
            Description = "Description",
            Price = 100,
            Image = "http://image.url",
            FoodStatus = FoodStatus.Combo, 
            Category = expectedCategory
        };

        // Setup mocks
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindSingleAsync(It.IsAny<Expression<Func<Food, bool>>>(), x => x.Category) )
            .ReturnsAsync(foodEntity);
        //_mockMapper.Setup(m => m.Map<FoodResponseCategory>(foodEntity)).Returns(expectedFoodResponseCategory);

        // Act
        var result = await _foodService.GetFoodById(foodId);

        // Assert
        result.Payload.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetFoodById_Error()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var expectedCategory = new Category { Id = Guid.NewGuid(), Name = "Category 1" };
        var foodEntity = new Food {
            Id = foodId,
            Name = "Food 1",
            Description = "Description",
            Price = 100,
            Image = "http://image.url",
            FoodStatus = FoodStatus.Combo, 
            Category = expectedCategory
        };

        // Setup mocks
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindSingleAsync(It.IsAny<Expression<Func<Food, bool>>>(), x => x.Category) )
            .ThrowsAsync(new Exception());
        //_mockMapper.Setup(m => m.Map<FoodResponseCategory>(foodEntity)).Returns(expectedFoodResponseCategory);

        // Act
        var result = await _foodService.GetFoodById(foodId);

        // Assert
        result.IsError.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetFoodById_NotFound()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var expectedCategory = new Category { Id = Guid.NewGuid(), Name = "Category 1" };
        var foodEntity = new Food {
            Id = foodId,
            Name = "Food 1",
            Description = "Description",
            Price = 100,
            Image = "http://image.url",
            FoodStatus = FoodStatus.Combo, 
            Category = expectedCategory
        };

        // Setup mocks
        _unitOfWorkMock.Setup(u => u.FoodRepository.FindSingleAsync(It.IsAny<Expression<Func<Food, bool>>>(), x => x.Category) )
            .ReturnsAsync((Food)null);
        //_mockMapper.Setup(m => m.Map<FoodResponseCategory>(foodEntity)).Returns(expectedFoodResponseCategory);

        // Act    
        var result = await _foodService.GetFoodById(foodId);

        // Assert
        result.IsError.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetFoodForSupplier_SuccessWithSearchTerm()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var searchTerm = "Pizza";
        var matchingFoods = new List<Food>
        {
            new Food { Id = Guid.NewGuid(), Name = "Pepperoni Pizza", SupplierCommissionRates = new List<SupplierCommissionRate>() },
            new Food { Id = Guid.NewGuid(), Name = "Vegetarian Pizza", SupplierCommissionRates = new List<SupplierCommissionRate>() }
        };
        // Add other relevant properties to your Food objects (IsDeleted, SupplierCommissionRate relationships, etc.)

        var nonMatchingFood = new Food { Id = Guid.NewGuid(), Name = "Pasta", SupplierCommissionRates = new List<SupplierCommissionRate> { new SupplierCommissionRate { SupplierId = supplierId } } };

        var allFoods = new List<Food> { matchingFoods[0], matchingFoods[1], nonMatchingFood };

        var expectedFoodResponses = new List<FoodResponse> 
        {
            // ... map matchingFoods to FoodResponse 
        };
        
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetFoodForSupplier(supplierId, searchTerm))
            .ReturnsAsync(matchingFoods);

        /*_mockMapper.Setup(m => m.Map<List<FoodResponse>>(matchingFoods))
            .Returns(expectedFoodResponses);*/

        // Act
        var result = await _foodService.GetFoodForSupplier(supplierId, searchTerm);

        // Assert
        result.Payload.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task GetFoodForSupplier_DatabaseRetrievalError()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var searchTerm = "Pizza";

        _unitOfWorkMock.Setup(u => u.FoodRepository.GetFoodForSupplier(supplierId, searchTerm))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _foodService.GetFoodForSupplier(supplierId, searchTerm);

        // Assert
        Assert.Contains("Database error", result.Errors.Select(e => e.Message));
    }
    
    private List<Food> GetFakeFoods()
    {
        return new List<Food>
        {
            new Food { Id = Guid.NewGuid(), Name = "Apple", IsDeleted = false },
            new Food { Id = Guid.NewGuid(), Name = "Banana", IsDeleted = false }
        };
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("apple")]
    public async Task GetFoodPaginationAsync_ReturnsCorrectData_WithAndWithoutSearchTerm(string searchTerm)
    {
        // Arrange
        var fakeFoods = new Pagination<Food>
        {
            Items = new List<Food>
            {
                new Food { Id = Guid.NewGuid(), Name = "Apple", IsDeleted = false },
                new Food { Id = Guid.NewGuid(), Name = "Banana", IsDeleted = false }
            },
            // TotalItems = 2,
            // TotalPages = 1,
            PageSize = 10,
            PageIndex = 0
        };

        var expectedResponse = new Pagination<FoodResponse>
        {
            Items = fakeFoods.Items.Select(f => new FoodResponse { Id = f.Id, Name = f.Name }).ToList(),
            // TotalItems = fakeFoods.TotalItems,
            // TotalPages = fakeFoods.TotalPages,
            PageSize = fakeFoods.PageSize,
            PageIndex = fakeFoods.PageIndex
        };

        _unitOfWorkMock.Setup(u => u.FoodRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Food, bool>>>()))
            .ReturnsAsync(fakeFoods);
        //_mapper.Map<Pagination<FoodResponse>>(It.IsAny<Pagination<Food>>()).Returns(expectedResponse);

        // Act
        var result = await _foodService.GetFoodPaginationAsync(searchTerm, 0, 10);

        // Assert
        Assert.NotNull(result.Payload);
        Assert.Equal(expectedResponse.Items.Count, result.Payload.Items.Count);
        Assert.Equal(0, result.Errors.Count);
    }
    
    [Fact]
    public async Task GetFoodPaginationAsync_HandlesExceptions()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.FoodRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Food, bool>>>()))
            .ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await _foodService.GetFoodPaginationAsync(null);

        // Assert
        Assert.NotNull(result.Errors);
        Assert.Single(result.Errors);
        Assert.Contains("An error occurred", result.Errors.First().Message);
    }
    
    [Fact]
    public async Task GetComboAndFoodsForPartner_ReturnsSuccess_WithData()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var dailyOrder = new DailyOrder
        {
            Id = dailyOrderId,
            MealSubscription = new MealSubscription
            {
                Meal = new Meal { MealType = "Lunch" },
                Company = new Company
                {
                    Name = "Tech Co",
                    PhoneNumber = "1234567890",
                    Email = "nu",
                    Address = "null"
                }
            },
            Orders = new List<Order>
            {
                new Order
                {
                    OrderStatus = OrderStatus.Paid,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            Quantity = 1,
                            Food = new Food { Id = Guid.NewGuid(), Name = "Burger" },
                            Combo = null
                        }
                    }
                }
            }
        };

        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(
                dailyOrderId,
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>()))
            .ReturnsAsync(dailyOrder);

        // Act
        var result = await _foodService.GetComboAndFoodsForPartner(dailyOrderId);

        // Assert
        Assert.NotNull(result.Payload);
        Assert.Equal("Tech Co", result.Payload.CompanyName);
        Assert.Single(result.Payload.TotalFoodResponses);
    }
    
    [Fact]
    public async Task GetComboAndFoodsForPartner_Handles_NotFoundIdException()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(
                dailyOrderId,
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>()))
            .ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _foodService.GetComboAndFoodsForPartner(dailyOrderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Id is not exist", result.Errors.First().Message);
    }
    
    [Fact]
    public async Task GetComboAndFoodsForPartner_Handles_Exception()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(
                dailyOrderId,
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>()))
            .ThrowsAsync(new Exception("General Failure"));

        // Act
        var result = await _foodService.GetComboAndFoodsForPartner(dailyOrderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains("General Failure", result.Errors.First().Message);
    }
    
    [Fact]
    public async Task GetComboAndFoodsForPartner_Handles_DailyOrderWithoutOrders()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var dailyOrder = new DailyOrder
        {
            Id = dailyOrderId,
            MealSubscription = new MealSubscription
            {
                Meal = new Meal { MealType = "Lunch" },
                Company = new Company
                {
                    Name = "Tech Co",
                    PhoneNumber = "1234567890",
                    Email = "null",
                    Address = "null"
                }
            },
            Orders = new List<Order>() // No orders
        };

        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(
                dailyOrderId,
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>(),
                It.IsAny<IncludeInfo<DailyOrder>>()))
            .ReturnsAsync(dailyOrder);

        // Act
        var result = await _foodService.GetComboAndFoodsForPartner(dailyOrderId);

        // Assert
        Assert.NotNull(result.Payload);
        Assert.Empty(result.Payload.TotalFoodResponses); // Expecting no food responses
    }
    

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task RemoveFood_FoodInComboOrMenu_ReturnsError(bool inCombo, bool inMenu)
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var food = new Food { Id = foodId, Name = "Sample Food" };
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ReturnsAsync(food);
        _unitOfWorkMock.Setup(x => x.ComboFoodRepository.IsFoodExistInCombo(foodId)).ReturnsAsync(inCombo);
        _unitOfWorkMock.Setup(x => x.MenuFoodRepository.IsFoodExistInMenu(foodId)).ReturnsAsync(inMenu);

        // Act
        var result = await _foodService.RemoveFood(foodId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("thuộc"));
    }

    [Fact]
    public async Task RemoveFood_SuccessfulDeletion_ReturnsSuccess()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var food = new Food { Id = foodId, Name = "Sample Food" };
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ReturnsAsync(food);
        _unitOfWorkMock.Setup(x => x.ComboFoodRepository.IsFoodExistInCombo(foodId)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.MenuFoodRepository.IsFoodExistInMenu(foodId)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.FoodRepository.SoftRemove(food));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _foodService.RemoveFood(foodId);

        // Assert
        Assert.False(result.IsError);
    }

    [Fact]
    public async Task RemoveFood_SaveChangesFailure_ReturnsError()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var food = new Food { Id = foodId, Name = "Sample Food" };
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ReturnsAsync(food);
        _unitOfWorkMock.Setup(x => x.ComboFoodRepository.IsFoodExistInCombo(foodId)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.MenuFoodRepository.IsFoodExistInMenu(foodId)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.FoodRepository.SoftRemove(food));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);

        // Act
        var result = await _foodService.RemoveFood(foodId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Xóa thất bại"));
    }

    [Fact]
    public async Task RemoveFood_ExceptionThrown_ReturnsUnknownError()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _foodService.RemoveFood(foodId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database failure"));
    }
    
    
    [Theory]
    [InlineData(2)]
    [InlineData(-1)]
    public async Task UpdateFood_InvalidFoodStatus_ReturnsError(int status)
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var food = new Food { Id = foodId, Name = "Original" };
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ReturnsAsync(food);

        var request = new UpdateFoodRequestModels { FoodStatus = status };

        // Act
        var result = await _foodService.UpdateFood(foodId, request);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Food status must be 0 or 1", result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdateFood_SuccessfulUpdate_WithImageUpload()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var food = new Food { Id = foodId, Name = "Original" };
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ReturnsAsync(food);
        _imgurServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("newImage.jpg");

        var request = new UpdateFoodRequestModels { Name = "Updated", Image = Mock.Of<IFormFile>() };

        // Act
        var result = await _foodService.UpdateFood(foodId, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("Updated", food.Name);
        Assert.Equal("newImage.jpg", food.Image);
    }

    [Fact]
    public async Task UpdateFood_SuccessfulUpdate_WithoutImageUpload()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var food = new Food { Id = foodId, Name = "Original", Image = "original.jpg" };
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ReturnsAsync(food);

        var request = new UpdateFoodRequestModels { Name = "Updated", Price = 9.99m };

        // Act
        var result = await _foodService.UpdateFood(foodId, request);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("Updated", food.Name);
        Assert.Equal(9.99m, food.Price);
        Assert.Equal("original.jpg", food.Image); // Image remains unchanged
    }

    [Fact]
    public async Task UpdateFood_HandlesException()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _foodService.UpdateFood(foodId, new UpdateFoodRequestModels());

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database failure"));
    }
    
    [Fact]
    public async Task GetComboAndFoodsForDelivery_ReturnsAggregatedDataSuccessfully()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        var dailyOrder = new DailyOrder
        {
            Id = dailyOrderId,
            MealSubscription = new MealSubscription
            {
                Meal = new Meal { MealType = "Lunch" },
                Company = new Company
                {
                    Name = "Tech Co",
                    PhoneNumber = "1234567890",
                    Email = "null",
                    Address = "null"
                }
            },
            Orders = new List<Order>
            {
                new Order
                {
                    OrderStatus = OrderStatus.Paid,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            Quantity = 1,
                            Food = new Food { Id = Guid.NewGuid(), Name = "Burger" },
                            Combo = null
                        }
                    }
                }
            }
        };

        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(
            dailyOrderId,
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>()))
        .ReturnsAsync(dailyOrder);

        // Act
        var result = await _foodService.GetComboAndFoodsForDelivery(dailyOrderId);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Single(result.Payload.TotalFoodResponses);
    }

    [Fact]
    public async Task GetComboAndFoodsForDelivery_DailyOrderNotFound_ReturnsNotFound()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(
            dailyOrderId,
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>()))
        .Throws(new NotFoundIdException());

        // Act
        var result = await _foodService.GetComboAndFoodsForDelivery(dailyOrderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Id is not exist", result.Errors.First().Message);
    }

    [Fact]
    public async Task GetComboAndFoodsForDelivery_HandlesExceptions()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(dailyOrderId,
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>(),
            It.IsAny<IncludeInfo<DailyOrder>>()))
        .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _foodService.GetComboAndFoodsForDelivery(dailyOrderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains("Unexpected error", result.Errors.First().Message);
    }
    
    [Fact]
    public async Task GetFoodsForPartner_PartnerNotFound_ReturnsError()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid());
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(new User()); // User without a partner

        // Act
        var result = await _foodService.GetFoodsForPartner(dailyOrderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("partner does not exist", result.Errors.First().Message);
    }

    [Fact]
    public async Task GetFoodsForPartner_CompanyDoesNotOwnOrder_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dailyOrderId = Guid.NewGuid();
        var user = new User
        {
            Partner = new Partner
            {
                Companies = new List<Company>
                {
                    new Company
                    {
                        Id = Guid.NewGuid(),
                        Name = "null",
                        PhoneNumber = "null",
                        Email = "null",
                        Address = "null"
                    }
                },
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0 // Company not matching the order's company
            }
        };
        var dailyOrder = new DailyOrder
        {
            MealSubscription = new MealSubscription
            {
                CompanyId = Guid.NewGuid() // Different company ID
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(dailyOrderId, It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>()))
            .ReturnsAsync(dailyOrder);

        // Act
        var result = await _foodService.GetFoodsForPartner(dailyOrderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Company doesn't have this daily order", result.Errors.First().Message);
    }

    [Fact]
    public async Task GetFoodsForPartner_SuccessfulDataRetrieval_ReturnsCorrectData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dailyOrderId = Guid.NewGuid();
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "null",
            PhoneNumber = "null",
            Email = "null",
            Address = "null"
        };
        var user = new User
        {
            Partner = new Partner
            {
                Companies = new List<Company>
                {
                    company
                },
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };
        var dailyOrder = new DailyOrder
        {
            Id = dailyOrderId,
            MealSubscription = new MealSubscription
            {
                Meal = new Meal { MealType = "Lunch" },
                Company = company,
                CompanyId = company.Id
            },
            Orders = new List<Order>()
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAndIncludeAsync(dailyOrderId, It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>(), It.IsAny<IncludeInfo<DailyOrder>>()))
            .ReturnsAsync(dailyOrder);

        // Act
        var result = await _foodService.GetFoodsForPartner(dailyOrderId);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(dailyOrder.MealSubscription.Meal.MealType, result.Payload.Meal);
    }

    [Fact]
    public async Task GetFoodsForPartner_HandlesException()
    {
        // Arrange
        var dailyOrderId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid());
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<IncludeInfo<User>>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _foodService.GetFoodsForPartner(dailyOrderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains("Unexpected error", result.Errors.First().Message);
    }
}