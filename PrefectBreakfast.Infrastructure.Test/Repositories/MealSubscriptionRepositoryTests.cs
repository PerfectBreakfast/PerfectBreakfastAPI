using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class MealSubscriptionRepositoryTests : SetupTest
{
    private readonly IMealSubscriptionRepository _mealSubscriptionRepository;

    public MealSubscriptionRepositoryTests()
    {
        _mealSubscriptionRepository =
            new MealSubscriptionRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task MealSubscriptionRepository_Should_ReturnCorrectData()
    {
        // arrange
        var mockData = _fixture.Build<MealSubscription>()
            .Without(x => x.Company)
            .Without(x => x.Meal)
            .Without(x => x.DailyOrders)
            .CreateMany(10).ToList();
        await _dbContext.MealSubscriptions.AddRangeAsync(mockData);
        await _dbContext.SaveChangesAsync();

        // act
        var result = await _mealSubscriptionRepository.GetAllAsync();

        // assert
        result.Should().BeEquivalentTo(mockData);
    }
}