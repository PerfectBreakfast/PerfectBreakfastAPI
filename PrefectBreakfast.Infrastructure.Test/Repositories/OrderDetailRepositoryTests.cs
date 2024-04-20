using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class OrderDetailRepositoryTests : SetupTest
{
    private readonly IOrderDetailRepository _orderDetailRepository;

    public OrderDetailRepositoryTests()
    {
        _orderDetailRepository =
            new OrderDetailRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task MealSubscriptionRepository_Should_ReturnCorrectData()
    {
        // arrange
        var mockData = _fixture.Build<OrderDetail>()
            .Without(x => x.Combo)
            .Without(x => x.Food)
            .Without(x => x.Order)
            .CreateMany(10).ToList();
        await _dbContext.OrderDetails.AddRangeAsync(mockData);
        await _dbContext.SaveChangesAsync();

        // act
        var result = await _orderDetailRepository.GetAllAsync();

        // assert
        result.Should().BeEquivalentTo(mockData);
    }
}