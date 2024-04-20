using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class ActionHistoryRepositoryTests : SetupTest
{
    private readonly IActionHistoryRepository _actionHistoryRepository;

    public ActionHistoryRepositoryTests()
    {
        _actionHistoryRepository = new ActionHistoryRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task ActionHistoryRepository_Should_ReturnCorrectData()
    {
        // arrange
        var mockData = _fixture.Build<ActionHistory>()
            .Without(x => x.DailyOrder)
            .Without(x => x.User)
            .CreateMany(10).ToList();
        await _dbContext.ActionHistories.AddRangeAsync(mockData);
        await _dbContext.SaveChangesAsync();

        // act
        var result = await _actionHistoryRepository.GetAllAsync();

        // assert
        result.Should().BeEquivalentTo(mockData);
    }
}