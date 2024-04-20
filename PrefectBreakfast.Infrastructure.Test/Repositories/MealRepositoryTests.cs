using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class MealRepositoryTests : SetupTest
{
    private readonly IMealRepository _mealRepository;

    public MealRepositoryTests()
    {
        _mealRepository = new MealRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    
    [Fact]
    public async Task MealRepository_Should_ReturnCorrectData()
    {
        // arrange
        var mockData = _fixture.Build<Meal>()
            .Without(x => x.MealSubscriptions)
            .CreateMany(10).ToList();
        await _dbContext.Meals.AddRangeAsync(mockData);
        await _dbContext.SaveChangesAsync();

        // act
        var result = await _mealRepository.GetAllAsync();

        // assert
        result.Should().BeEquivalentTo(mockData);
    }
}