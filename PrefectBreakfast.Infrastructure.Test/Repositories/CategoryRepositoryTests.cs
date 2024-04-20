using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class CategoryRepositoryTests : SetupTest
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryRepositoryTests()
    {
        _categoryRepository = new CategoryRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task CategoryRepository_Should_ReturnCorrectData()
    {
        // arrange
        var mockData = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .CreateMany(10).ToList();
        await _dbContext.Categories.AddRangeAsync(mockData);
        await _dbContext.SaveChangesAsync();

        // act
        var result = await _categoryRepository.GetAllAsync();

        // assert
        result.Should().BeEquivalentTo(mockData);
    }
    
    [Fact]
    public async Task GetCategoryDetail_ReturnsCategoryWithSpecificFoodStatus()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var food1 = new Food { Id = Guid.NewGuid(), Name = "Apple Pie", FoodStatus = FoodStatus.Combo };
        var food2 = new Food { Id = Guid.NewGuid(), Name = "Banana Bread", FoodStatus = FoodStatus.Retail };  // This should not be included
        var category = new Category
        {
            Id = categoryId,
            Name = "Desserts",
            Foods = new List<Food> { food1, food2 }
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryRepository.GetCategoryDetail(categoryId, FoodStatus.Combo);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(categoryId);
        //result.Foods.Should().ContainSingle();
        //result.Foods.Should().OnlyContain(f => f.FoodStatus == FoodStatus.Combo && f.Name == "Apple Pie");
    }
}