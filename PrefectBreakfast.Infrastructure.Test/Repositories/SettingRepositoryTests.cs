using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class SettingRepositoryTests : SetupTest
{
    private readonly ISettingRepository _settingRepository;

    public SettingRepositoryTests()
    {
        _settingRepository = new SettingRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task MealSubscriptionRepository_Should_ReturnCorrectData()
    {
        // arrange
        var mockData = _fixture.Build<Setting>()
            .Create();
        await _dbContext.Settings.AddAsync(mockData);
        await _dbContext.SaveChangesAsync();

        // act
        var result = await _settingRepository.GetAllAsync();

        // assert
        result.Should().HaveCount(1);
    }
}