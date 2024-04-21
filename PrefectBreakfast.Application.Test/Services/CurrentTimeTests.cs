using FluentAssertions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Services;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class CurrentTimeTests : SetupTest
{
    private readonly ICurrentTime _currentTime;

    public CurrentTimeTests()
    {
        _currentTime = new CurrentTime();
    }

    [Fact]
    public void GetCurrentTime_ShouldReturnTimeExaclyToTheMiliSec()
    {
        //arrange
        var expectedTime = DateTime.UtcNow.AddHours(7);

        //act
        var result = _currentTime.GetCurrentTime();

        //assert
        TimeSpan difference = result - expectedTime;
        var timeDiffLessThan1MiliSec = difference < TimeSpan.FromMilliseconds(1);
        timeDiffLessThan1MiliSec.Should().BeTrue();
    }
}