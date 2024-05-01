using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.DailyOrder.Request;
using PerfectBreakfast.Application.Models.DailyOrder.Response;
using PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.StatisticResponse;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class DailyOrderControllerTest : SetupTest
{
    private readonly DailyOrderController _dailyOrderController;

    public DailyOrderControllerTest()
    {
        _dailyOrderController = new DailyOrderController(_dailyOrderServiceMock.Object, _exportExcelServiceMock.Object,
            _supplierFoodAssignmentServiceMock.Object);
    }
    
    [Fact]
    public async Task GetDailyOrders_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<DailyOrderResponse>>
        {
            Payload = new List<DailyOrderResponse>(),
            IsError = false
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrders()).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDailyOrders_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<DailyOrderResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrders()).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrders();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetDailyOrderPagination_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var status = DailyOrderStatus.Initial;
        var mockResponse = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>
        {
            Payload = new Pagination<DailyOrderForDeliveryResponse>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 100,
                Items = new List<DailyOrderForDeliveryResponse>()
            },
            IsError = false
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderPaginationAsync(status, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderPagination(pageIndex, pageSize, status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDailyOrderPagination_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var status = DailyOrderStatus.Initial;
        var mockResponse = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error during pagination" } }
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderPaginationAsync(status, pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderPagination(pageIndex, pageSize, status);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task CompleteDailyOrder_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<bool>
        {
            Payload = true,
            IsError = false
        };
        _dailyOrderServiceMock.Setup(x => x.CompleteDailyOrder(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.CompleteDailyOrder(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);
    }

    [Fact]
    public async Task CompleteDailyOrder_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<bool>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to complete order" } }
        };
        _dailyOrderServiceMock.Setup(x => x.CompleteDailyOrder(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.CompleteDailyOrder(id);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetDailyOrderProcessingByPartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DailyOrderForPartnerResponse>>
        {
            Payload = new Pagination<DailyOrderForPartnerResponse>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 50,
                Items = new List<DailyOrderForPartnerResponse>()
            },
            IsError = false
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderProcessingByPartner(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderProcessingByPartner(pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDailyOrderProcessingByPartner_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DailyOrderForPartnerResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderProcessingByPartner(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderProcessingByPartner(pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetDailyOrderProcessingByDelivery_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>
        {
            Payload = new Pagination<DailyOrderForDeliveryResponse>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 20,
                Items = new List<DailyOrderForDeliveryResponse>()
            },
            IsError = false
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderProcessingByDelivery(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderProcessingByDelivery(pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDailyOrderProcessingByDelivery_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderProcessingByDelivery(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderProcessingByDelivery(pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetDailyOrderByPartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DailyOrderForPartnerResponse>>
        {
            Payload = new Pagination<DailyOrderForPartnerResponse>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 50,
                Items = new List<DailyOrderForPartnerResponse>()
            },
            IsError = false
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderByPartner(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderByPartner(pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDailyOrderByPartner_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DailyOrderForPartnerResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error fetching data" } }
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderByPartner(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderByPartner(pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetDailyOrderStatistic_ReturnsFile_WhenNoErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<List<DailyOrderStatisticResponse>>
        {
            Payload = new List<DailyOrderStatisticResponse>(),
            IsError = false
        };
        var mockContent = new byte[] { 0x1, 0x2, 0x3 };

        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderForDownload(fromDate, toDate)).ReturnsAsync(mockResponse);
        _exportExcelServiceMock.Setup(x => x.DownloadDailyOrderStatistic(mockResponse.Payload)).Returns(mockContent);

        // Act
        var result = await _dailyOrderController.GetDailyOrderStatistic(fromDate, toDate);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
    }

    [Fact]
    public async Task GetDailyOrderStatistic_ReturnsNotFound_WhenNoData()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var mockResponse = new OperationResult<List<DailyOrderStatisticResponse>>
        {
            Payload = new List<DailyOrderStatisticResponse>(),
            IsError = false
        };

        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderForDownload(fromDate, toDate)).ReturnsAsync(mockResponse);
        _exportExcelServiceMock.Setup(x => x.DownloadDailyOrderStatistic(mockResponse.Payload)).Returns((byte[])null);

        // Act
        var result = await _dailyOrderController.GetDailyOrderStatistic(fromDate, toDate);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetDailyOrderStatisticForSuperAdmin_ReturnsFile_WhenNoErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var dailyOrderResponse = new OperationResult<List<DailyOrderStatisticResponse>>
        {
            Payload = new List<DailyOrderStatisticResponse>(),
            IsError = false
        };
        var supplierFoodResponse = new OperationResult<List<SupplierFoodAssignmentForSuperAdmin>>
        {
            Payload = new List<SupplierFoodAssignmentForSuperAdmin>(),
            IsError = false
        };
        byte[] mockContent = new byte[] { 0x1, 0x2, 0x3 };

        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderForDownload(fromDate, toDate)).ReturnsAsync(dailyOrderResponse);
        _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentsForSuperAdmin(fromDate, toDate)).ReturnsAsync(supplierFoodResponse);
        _exportExcelServiceMock.Setup(x => x.DownloadDailyOrderStatisticForSuperAdmin(dailyOrderResponse.Payload, supplierFoodResponse.Payload)).Returns(mockContent);

        // Act
        var result = await _dailyOrderController.GetDailyOrderStatisticForSuperAdmin(fromDate, toDate);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
        Assert.Equal($"Thống kê đơn hàng từ {fromDate} đến {toDate}.xlsx", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task GetDailyOrderStatisticForSuperAdmin_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var response = new OperationResult<List<DailyOrderStatisticResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Error retrieving data" } }
        };

        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderForDownload(fromDate, toDate)).ReturnsAsync(response);

        // Act
        var result = await _dailyOrderController.GetDailyOrderStatisticForSuperAdmin(fromDate, toDate);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);  // Assuming HandleErrorResponse is setup to return 500 on server errors
    }

    [Fact]
    public async Task GetDailyOrderByDelivery_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>
        {
            Payload = new Pagination<DailyOrderForDeliveryResponse>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = 20,
                Items = new List<DailyOrderForDeliveryResponse>()
            },
            IsError = false
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderByDelivery(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderByDelivery(pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetDailyOrderByDelivery_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } }
        };
        _dailyOrderServiceMock.Setup(x => x.GetDailyOrderByDelivery(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _dailyOrderController.GetDailyOrderByDelivery(pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

}