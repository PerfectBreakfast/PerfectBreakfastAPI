using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class DashboardControllerTest : SetupTest
{
    private readonly DashboardController _dashboardController;

    public DashboardControllerTest()
    {
        _dashboardController = new DashboardController(_dashboardServiceMock.Object);
    }
    
    [Fact]
    public async Task TopComboAndFood_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<List<TopComboAndFoodResponse>>
        {
            Payload = new List<TopComboAndFoodResponse>(),
            IsError = false
        };
        _dashboardServiceMock.Setup(x => x.GetTopComboAndFood(fromDate, toDate)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dashboardController.TopComboAndFood(fromDate, toDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task TopComboAndFood_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<List<TopComboAndFoodResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Failed to fetch data" } }
        };
        _dashboardServiceMock.Setup(x => x.GetTopComboAndFood(fromDate, toDate)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dashboardController.TopComboAndFood(fromDate, toDate);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetTopCompany_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<List<CompanyStatistic>>
        {
            Payload = new List<CompanyStatistic>(),
            IsError = false
        };
        _dashboardServiceMock.Setup(x => x.GetTop10CompanyOrder(fromDate, toDate)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dashboardController.GetTopCompany(fromDate, toDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetTopCompany_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<List<CompanyStatistic>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Data not found" } }
        };
        _dashboardServiceMock.Setup(x => x.GetTop10CompanyOrder(fromDate, toDate)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dashboardController.GetTopCompany(fromDate, toDate);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOrderStatistic_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var fromDateData = DateOnly.Parse("2024-01-15");
        var toDateData = DateOnly.Parse("2024-01-15");
        var mockResponse = new OperationResult<OrderStatisticResponse>
        {
            Payload = new OrderStatisticResponse(fromDateData, toDateData, 1, 1, 1, 1, 1),
            IsError = false
        };
        _dashboardServiceMock.Setup(x => x.OrderStatistic(fromDate, toDate)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dashboardController.GetOrderStatistic(fromDate, toDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetOrderStatistic_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<OrderStatisticResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving order statistics" } }
        };
        _dashboardServiceMock.Setup(x => x.OrderStatistic(fromDate, toDate)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dashboardController.GetOrderStatistic(fromDate, toDate);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetOrderByDateStatistic_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<List<OrderByDateStatistic>>
        {
            Payload = new List<OrderByDateStatistic>(),
            IsError = false
        };
        _dashboardServiceMock.Setup(x => x.OrderAmountByDateStatistic(fromDate, toDate)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dashboardController.GetOrderByDateStatistic(fromDate, toDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetOrderByDateStatistic_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<List<OrderByDateStatistic>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Data not found" } }
        };
        _dashboardServiceMock.Setup(x => x.OrderAmountByDateStatistic(fromDate, toDate)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dashboardController.GetOrderByDateStatistic(fromDate, toDate);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    
}