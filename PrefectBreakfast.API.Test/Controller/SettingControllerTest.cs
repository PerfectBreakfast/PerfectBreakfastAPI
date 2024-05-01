using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SettingModels.Request;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class SettingControllerTest : SetupTest
{
    private readonly SettingController _settingController;

    public SettingControllerTest()
    {
        _settingController = new SettingController(_hangfireSettingServiceMock.Object);
    }
    
    [Fact]
    public async Task SetupTimeAutoCreateAndUpdateDailyOrder_ReturnsOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        Random random = new Random();

        // Tạo giờ, phút và giây ngẫu nhiên
        int hours = random.Next(24); // Giá trị từ 0 đến 23
        int minutes = random.Next(60); // Giá trị từ 0 đến 59
        int seconds = random.Next(60); // Giá trị từ 0 đến 59

        // Tạo đối tượng TimeOnly từ giờ, phút và giây ngẫu nhiên
        var timeValue = new TimeOnly(hours, minutes, seconds); 
        var request = new TimeSettingRequest(timeValue);
        var mockResponse = new OperationResult<TimeOnly> { IsError = false, Payload = TimeOnly.Parse("13:00") };
        _hangfireSettingServiceMock.Setup(x => x.UpdateTime(Guid.NewGuid(), request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _settingController.SetupTimeAutoCreateAndUpdateDailyOrder(Guid.NewGuid(), request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task SetupTimeAutoCreateAndUpdateDailyOrder_ReturnsError_WhenUpdateFails()
    {
        // Arrange
        Random random = new Random();

        // Tạo giờ, phút và giây ngẫu nhiên
        int hours = random.Next(24); // Giá trị từ 0 đến 23
        int minutes = random.Next(60); // Giá trị từ 0 đến 59
        int seconds = random.Next(60); // Giá trị từ 0 đến 59

        // Tạo đối tượng TimeOnly từ giờ, phút và giây ngẫu nhiên
        var timeValue = new TimeOnly(hours, minutes, seconds); 
        var request = new TimeSettingRequest(timeValue);
        var mockResponse = new OperationResult<TimeOnly> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Not found" } } };
        _hangfireSettingServiceMock.Setup(x => x.UpdateTime(Guid.NewGuid(), request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _settingController.SetupTimeAutoCreateAndUpdateDailyOrder(Guid.NewGuid(), request);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal(mockResponse.Errors, badRequestResult.Value);
    }

    [Fact]
    public async Task GetTimeAuto_ReturnsOk_WhenFetchedSuccessfully()
    {
        // Arrange
        var mockResponse = new OperationResult<Setting> { IsError = false, Payload = new Setting { Time = TimeOnly.Parse("10:00") } };
        _hangfireSettingServiceMock.Setup(x => x.GetTimeAuto()).ReturnsAsync(mockResponse);

        // Act
        var result = await _settingController.GetTimeAuto();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetTimeAuto_ReturnsError_WhenFetchFails()
    {
        // Arrange
        var mockResponse = new OperationResult<Setting> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } } };
        _hangfireSettingServiceMock.Setup(x => x.GetTimeAuto()).ReturnsAsync(mockResponse);

        // Act
        var result = await _settingController.GetTimeAuto();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
}