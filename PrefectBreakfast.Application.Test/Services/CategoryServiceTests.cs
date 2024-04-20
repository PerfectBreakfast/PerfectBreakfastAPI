using AutoFixture;
using FluentAssertions;
using Moq;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class CategoryServiceTests : SetupTest
{
    private readonly ICategoryService _categoryService;

    public CategoryServiceTests()
    {
        _categoryService = new CategoryService(_unitOfWorkMock.Object, _mapper);
    }
    
    [Fact]
    public async Task GetCategoryId_ReturnsCategoryDetailFood_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .Create();
        var expectedResult = _mapper.Map<CategoryResponse>(category);
        _unitOfWorkMock.Setup(x => x.CategoryRepository.GetCategoryDetail(categoryId, FoodStatus.Combo))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetCategoryId(categoryId, FoodStatus.Combo);

        // Assert
        Assert.NotNull(result.Payload);
        _unitOfWorkMock.Verify(x => x.CategoryRepository.GetCategoryDetail(categoryId, FoodStatus.Combo), Times.Once);
        result.Payload.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task GetCategoryId_ReturnsError_WhenCategoryIdDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        
        _unitOfWorkMock.Setup(x => x.CategoryRepository.GetCategoryDetail(categoryId, FoodStatus.Combo))
            .ThrowsAsync(new InvalidOperationException(""));

        // Act
        var result = await _categoryService.GetCategoryId(categoryId, FoodStatus.Combo);

        // Assert
        Assert.Null(result.Payload);
        Assert.True(result.IsError);
        _unitOfWorkMock.Verify(x => x.CategoryRepository.GetCategoryDetail(categoryId, FoodStatus.Combo), Times.Once);
    }
    
    [Fact]
    public async Task GetCategoryId_ReturnsError_WhenCategoryIdDoesNotExist2()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        
        _unitOfWorkMock.Setup(x => x.CategoryRepository.GetCategoryDetail(categoryId, FoodStatus.Combo))
            .ThrowsAsync(new NotFoundIdException(""));

        // Act
        var result = await _categoryService.GetCategoryId(categoryId, FoodStatus.Combo);

        // Assert
        Assert.Null(result.Payload);
        Assert.True(result.IsError);
        _unitOfWorkMock.Verify(x => x.CategoryRepository.GetCategoryDetail(categoryId, FoodStatus.Combo), Times.Once);
    }
    
    [Fact]
    public async Task CreateCategory_AddsAndReturnsCategory_WhenValidRequest()
    {
        // Arrange
        var category = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .Create();
        var requestModel = _mapper.Map<CreateCategoryRequest>(category);
        var expectedResult = _mapper.Map<CategoryResponse>(category);
        
        _unitOfWorkMock.Setup(u => u.CategoryRepository.AddAsync(It.IsAny<Category>())).ReturnsAsync(category);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1); // Assume successful save

        // Act
        var result = await _categoryService.CreateCategory(requestModel);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.CategoryRepository.AddAsync(It.IsAny<Category>()), Times.Once());
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once());
        result.Payload.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task CreateCategory_HandlesExceptionsProperly()
    {
        // Arrange
        var requestModel = _fixture.Create<CreateCategoryRequest>();
        var exceptionMessage = "Database connection failed";

        _unitOfWorkMock.Setup(u => u.CategoryRepository.AddAsync(It.IsAny<Category>()))
            .ThrowsAsync(new InvalidOperationException(exceptionMessage));

        // Act
        var result = await _categoryService.CreateCategory(requestModel);

        // Assert
        Assert.True(result.IsError);
        Assert.Null(result.Payload);
    }
    
    [Fact]
    public async Task CreateCategory_FailsToSaveChanges()
    {
        // Arrange
        var category = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .Create();
        var requestModel = _mapper.Map<CreateCategoryRequest>(category);
    
        _unitOfWorkMock.Setup(u => u.CategoryRepository.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ThrowsAsync(new Exception());  // Simulate failure to save changes

        // Act
        var result = await _categoryService.CreateCategory(requestModel);

        // Assert
        Assert.True(result.IsError);
        Assert.Null(result.Payload);
        Assert.Single(result.Errors);
    }
    
    [Fact]
    public async Task GetAllCategorys_ReturnsAllCategories_WhenCategoriesExist()
    {
        // Arrange
        var categories = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .CreateMany(5)
            .ToList();
        var expectedResults = _mapper.Map<List<CategoryResponse>>(categories);

        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetAllAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _categoryService.GetAllCategorys();

        // Assert
        //result.Success.Should().BeTrue();
        result.Payload.Should().BeEquivalentTo(expectedResults);
        _unitOfWorkMock.Verify(x => x.CategoryRepository.GetAllAsync(), Times.Once());
    }
    
    [Fact]
    public async Task GetAllCategorys_HandlesException_WhenErrorOccurs()
    {
        // Arrange
        var exceptionMessage = "Database failed";
        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetAllAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _categoryService.GetAllCategorys();

        // Assert
        //Assert.False(result.IsSuccess);
        result.IsError.Should().BeTrue();
        Assert.Null(result.Payload);
        //Assert.Contains(exceptionMessage, result.Errors);
    }
    
    [Fact]
    public async Task RemoveCategory_RemovesCategorySuccessfully_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .With(x => x.Id, categoryId)
            .Create();

        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _unitOfWorkMock.Setup(u => u.CategoryRepository.Remove(category))
            .Returns(category);

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _categoryService.RemoveCategory(categoryId);

        // Assert
        //result.Success.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.CategoryRepository.GetByIdAsync(categoryId), Times.Once());
        _unitOfWorkMock.Verify(x => x.CategoryRepository.Remove(category), Times.Once());
        result.Payload.Should().BeEquivalentTo(_mapper.Map<CategoryResponse>(category));
    }
    
    [Fact]
    public async Task RemoveCategory_ReturnsError_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(categoryId))
            .ThrowsAsync(new InvalidOperationException(""));

        // Act
        var result = await _categoryService.RemoveCategory(categoryId);

        // Assert
        Assert.Null(result.Payload);
        Assert.Single(result.Errors);
    }
    
    [Fact]
    public async Task UpdateCategory_UpdatesAndReturnsCategory_WhenValidRequest()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .With(x => x.Id, categoryId)
            .Create();
        var updateRequest = new UpdateCategoryRequest { Name = "Updated Name" };

        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _categoryService.UpdateCategory(categoryId, updateRequest);

        // Assert
        result.Payload.Name.Should().Be(updateRequest.Name);
        _unitOfWorkMock.Verify(x => x.CategoryRepository.Update(It.IsAny<Category>()), Times.Once());
    }
    
    [Fact]
    public async Task UpdateCategory_HandlesNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var updateRequest = new UpdateCategoryRequest { Name = "Updated Name" };

        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(categoryId))
            .ThrowsAsync(new InvalidOperationException(""));

        // Act
        var result = await _categoryService.UpdateCategory(categoryId, updateRequest);

        // Assert
        Assert.Null(result.Payload);
        Assert.Single(result.Errors);
    }
}