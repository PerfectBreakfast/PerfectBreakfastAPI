using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class SupplierFoodAssignmentControllerTest : SetupTest
{
    private readonly SupplierFoodAssignmentController _supplierFoodAssignmentController;

    public SupplierFoodAssignmentControllerTest()
    {
        _supplierFoodAssignmentController =
            new SupplierFoodAssignmentController(_supplierFoodAssignmentServiceMock.Object, _exportExcelServiceMock.Object);
    }
    
    [Fact]
    public async Task CreateSupplierFoodAssignment_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var request = new SupplierFoodAssignmentsRequest();
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _supplierFoodAssignmentServiceMock.Setup(x => x.CreateSupplierFoodAssignment(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.CreateSupplierFoodAssignment(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateSupplierFoodAssignment_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var request = new SupplierFoodAssignmentsRequest();
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to create supplier food assignment." } } };
        _supplierFoodAssignmentServiceMock.Setup(x => x.CreateSupplierFoodAssignment(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.CreateSupplierFoodAssignment(request);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentBySupplier_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<SupplierFoodAssignmentForSupplier>> { Payload = new Pagination<SupplierFoodAssignmentForSupplier>(), IsError = false };
        _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentBySupplier(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.GetSupplierFoodAssignmentBySupplier(pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentBySupplier_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<SupplierFoodAssignmentForSupplier>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to fetch supplier food assignments." } } };
        _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentBySupplier(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.GetSupplierFoodAssignmentBySupplier(pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentByPartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<SupplierFoodAssignmentForPartner>> { Payload = new Pagination<SupplierFoodAssignmentForPartner>(), IsError = false };
        _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentByPartner(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.GetSupplierFoodAssignmentByPartner(pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentByPartner_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var mockResponse = new OperationResult<Pagination<SupplierFoodAssignmentForPartner>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to fetch supplier food assignments." } } };
        _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentByPartner(pageIndex, pageSize)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.GetSupplierFoodAssignmentByPartner(pageIndex, pageSize);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task ChangeStatusFoodAssignment_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = 1;
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _supplierFoodAssignmentServiceMock.Setup(x => x.ChangeStatusFoodAssignment(id, status)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.ConfirmFoodAssignmentBySupplier(id, status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task ChangeStatusFoodAssignment_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = 1;
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to change status of food assignment." } } };
        _supplierFoodAssignmentServiceMock.Setup(x => x.ChangeStatusFoodAssignment(id, status)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.ConfirmFoodAssignmentBySupplier(id, status);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task CompleteFoodAssignment_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<FoodAssignmentResponse> { Payload = new FoodAssignmentResponse(), IsError = false };
        _supplierFoodAssignmentServiceMock.Setup(x => x.CompleteFoodAssignment(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.CompleteFoodAssignmentByPartner(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CompleteFoodAssignment_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mockResponse = new OperationResult<FoodAssignmentResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to complete food assignment." } } };
        _supplierFoodAssignmentServiceMock.Setup(x => x.CompleteFoodAssignment(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.CompleteFoodAssignmentByPartner(id);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateSupplierCommissionRate_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var request = new UpdateSupplierFoodAssignment(Guid.NewGuid(), Guid.NewGuid());
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _supplierFoodAssignmentServiceMock.Setup(x => x.UpdateSupplierFoodAssignment(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.UpdateSupplierCommissionRate(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateSupplierCommissionRate_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var request = new UpdateSupplierFoodAssignment(Guid.NewGuid(), Guid.NewGuid());
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Failed to update supplier commission rate." } } };
        _supplierFoodAssignmentServiceMock.Setup(x => x.UpdateSupplierFoodAssignment(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.UpdateSupplierCommissionRate(request);

        // Assert
        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, errorResult.StatusCode);
    }

    [Fact]
    public async Task DownloadSupplierFoodAssignment_ReturnsFile_WhenNoErrors()
    {
        // Arrange
        var request = new DownloadSupplierFoodAssignmentRequest(DateOnly.MaxValue, new List<Guid>());
        var mockResponse = new OperationResult<List<FoodAssignmentDownloadForSupplier>> { Payload = new List<FoodAssignmentDownloadForSupplier>(), IsError = false };
        _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentsForSupplierDownload(request.PackageIds)).ReturnsAsync(mockResponse);
        var mockExcelContent = new byte[10]; // Mock excel content
        _exportExcelServiceMock.Setup(x => x.DownloadSupplierFoodAssignmentForSupplier(mockResponse.Payload)).Returns(mockExcelContent);

        // Act
        var result = await _supplierFoodAssignmentController.DownloadSupplierFoodAssignment(request);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
        Assert.NotNull(fileResult.FileDownloadName);
    }

    [Fact]
    public async Task DownloadSupplierFoodAssignment_ReturnsNotFound_WhenServiceErrors()
    {
        // Arrange
        var request = new DownloadSupplierFoodAssignmentRequest(DateOnly.MaxValue, new List<Guid>());
        var mockResponse = new OperationResult<List<FoodAssignmentDownloadForSupplier>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No supplier food assignments found." } } };
        _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentsForSupplierDownload(request.PackageIds)).ReturnsAsync(mockResponse);

        // Act
        var result = await _supplierFoodAssignmentController.DownloadSupplierFoodAssignment(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
    
    [Fact]
        public async Task ConfirmAndCompleteAllFoodAssignments_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var packageId = Guid.NewGuid();
            var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
            _supplierFoodAssignmentServiceMock.Setup(x => x.ConfirmAllFoodAssignments(packageId)).ReturnsAsync(mockResponse);

            // Act
            var result = await _supplierFoodAssignmentController.ConfirmAndCompleteAllFoodAssignments(packageId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
            _supplierFoodAssignmentServiceMock.Verify(x => x.ConfirmAllFoodAssignments(packageId), Times.Once);
        }

        // Unit test for AcceptAndCompleteAllFoodAssignments
        [Fact]
        public async Task AcceptAndCompleteAllFoodAssignments_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var request = new FoodAssignmentForPartner();
            var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
            _supplierFoodAssignmentServiceMock.Setup(x => x.AcceptAllCompleteFoodAssignments(request)).ReturnsAsync(mockResponse);

            // Act
            var result = await _supplierFoodAssignmentController.AcceptAndCompleteAllFoodAssignments(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
            _supplierFoodAssignmentServiceMock.Verify(x => x.AcceptAllCompleteFoodAssignments(request), Times.Once);
        }
        
        [Fact]
        public async Task DownloadSupplierFoodAssignmentForPartner_ReturnsOk_WhenNoErrors()
        {
            // Arrange
            var request = new DownloadFoodAssignmentForPartnerRequest(DateOnly.MinValue, new List<Guid>(), Guid.NewGuid());
            var mockResponse = new OperationResult<List<FoodAssignmentDownloadForSupplier>> { IsError = false, Payload = new List<FoodAssignmentDownloadForSupplier>() };
            _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentsForPartnerDownload(request.PackageIds, request.SupplierId)).ReturnsAsync(mockResponse);

            // Act
            var result = await _supplierFoodAssignmentController.DownloadSupplierFoodAssignmentForPartner(request);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
            Assert.NotNull(fileResult.FileDownloadName);
        }
        
        // Unit test for DownloadSupplierFoodAssignmentForPartner
        [Fact]
        public async Task DownloadSupplierFoodAssignmentForPartner_ReturnsNotFound_WhenServiceErrors()
        {
            // Arrange
            var request = new DownloadFoodAssignmentForPartnerRequest(DateOnly.MinValue, new List<Guid>(), Guid.NewGuid());
            var mockResponse = new OperationResult<List<FoodAssignmentDownloadForSupplier>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No supplier food assignments found." } } };
            _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentsForPartnerDownload(request.PackageIds, request.SupplierId)).ReturnsAsync(mockResponse);

            // Act
            var result = await _supplierFoodAssignmentController.DownloadSupplierFoodAssignmentForPartner(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        // Unit test for GetSupplierFoodAssignmentDetailBySupplier
        [Fact]
        public async Task GetSupplierFoodAssignmentDetailBySupplier_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var packageIds = new List<Guid> { Guid.NewGuid() };
            var mockResponse = new OperationResult<List<FoodAssignmentGroupByCompany>> { IsError = false, Payload = new List<FoodAssignmentGroupByCompany>() };
            _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentDetailBySupplier(packageIds)).ReturnsAsync(mockResponse);

            // Act
            var result = await _supplierFoodAssignmentController.GetSupplierFoodAssignmentDetailBySupplier(packageIds);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _supplierFoodAssignmentServiceMock.Verify(x => x.GetSupplierFoodAssignmentDetailBySupplier(packageIds), Times.Once);
        }

        // Unit test for GetSupplierFoodAssignmentDetailByPartner
        [Fact]
        public async Task GetSupplierFoodAssignmentDetailByPartner_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var request = new FoodAssignmentByPartnerRequest(Guid.NewGuid(), new List<Guid>());
            var mockResponse = new OperationResult<List<FoodAssignmentGroupByCompany>> { IsError = false, Payload = new List<FoodAssignmentGroupByCompany>() };
            _supplierFoodAssignmentServiceMock.Setup(x => x.GetSupplierFoodAssignmentDetailByPartner(request)).ReturnsAsync(mockResponse);

            // Act
            var result = await _supplierFoodAssignmentController.GetSupplierFoodAssignmentDetailByPartner(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _supplierFoodAssignmentServiceMock.Verify(x => x.GetSupplierFoodAssignmentDetailByPartner(request), Times.Once);
        }
    
}