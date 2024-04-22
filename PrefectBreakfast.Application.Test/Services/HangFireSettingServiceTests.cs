using System.Linq.Expressions;
using Hangfire;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SettingModels.Request;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class HangFireSettingServiceTests : SetupTest
{
    private readonly IHangfireSettingService _hangfireSettingService;

    public HangFireSettingServiceTests()
    {
        _hangfireSettingService = new HangFireSettingService(_recurringJobManagerMock.Object, _unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task UpdateTime_UpdatesSettingAndSchedulesJob_WhenIdExists()
    {
        // Arrange
        var settingId = Guid.NewGuid();
        var setting = new Setting { Id = settingId, Time = TimeOnly.FromDateTime(DateTime.Now) };
        var request = new TimeSettingRequest ( TimeOnly.FromDateTime(DateTime.Now.AddHours(2)) );

        _unitOfWorkMock.Setup(u => u.SettingRepository.GetByIdAsync(settingId)).ReturnsAsync(setting);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _hangfireSettingService.UpdateTime(settingId, request);

        // Assert
        Assert.Equal(request.Time, result.Payload);
        /*_recurringJobManagerMock.Verify(m => m.AddOrUpdate<IManagementService>(
                It.IsAny<string>(), 
                It.IsAny<Expression<Action<IManagementService>>>(), 
                It.IsAny<string>()), 
            Times.Once);*/
    }
    
    [Fact]
    public async Task UpdateTime_HandlesExceptions()
    {
        // Arrange
        var settingId = Guid.NewGuid();
        var request = new TimeSettingRequest (TimeOnly.FromDateTime(DateTime.Now.AddHours(2)) );

        _unitOfWorkMock.Setup(u => u.SettingRepository.GetByIdAsync(settingId)).ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _hangfireSettingService.UpdateTime(settingId, request);

        // Assert
        //Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Message == "Database failure");
    }
    
    [Fact]
    public async Task GetTimeAuto_ReturnsFirstSetting_WhenSettingsExist()
    {
        // Arrange
        var settings = new List<Setting> { new Setting { Id = Guid.NewGuid(), Time = TimeOnly.FromDateTime(DateTime.Now) } };
        _unitOfWorkMock.Setup(u => u.SettingRepository.GetAllAsync()).ReturnsAsync(settings);

        // Act
        var result = await _hangfireSettingService.GetTimeAuto();

        // Assert
        //Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);
        Assert.Equal(settings.First(), result.Payload);
    }
    
    [Fact]
    public async Task GetTimeAuto_HandlesExceptions()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.SettingRepository.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _hangfireSettingService.GetTimeAuto();

        // Assert
        //Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Message == "Database error");
    }
}