using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class UserControllerTest : SetupTest
{
    private readonly UserController _userController;

    public UserControllerTest()
    {
        _userController = new UserController(_userServiceMock.Object);
    }
    
    [Fact]
    public async Task GetUsers_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var mockResponse = new OperationResult<List<UserResponse>> { IsError = false, Payload = new List<UserResponse>() };
        _userServiceMock.Setup(x => x.GetUsers()).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetUsers_ReturnsNotFound_WhenServiceFails()
    {
        // Arrange
        var mockResponse = new OperationResult<List<UserResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No users found." } } };
        _userServiceMock.Setup(x => x.GetUsers()).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetUsers();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async Task GetUser_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockResponse = new OperationResult<UserResponse> { IsError = false, Payload = new UserResponse() };
        _userServiceMock.Setup(x => x.GetUser(userId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetUser(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenServiceFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockResponse = new OperationResult<UserResponse> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "User not found." } } };
        _userServiceMock.Setup(x => x.GetUser(userId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetUser(userId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
    
    [Fact]
    public async Task GetUserPagination_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var searchTerm = "test";
        var mockResponse = new OperationResult<Pagination<UserModelResponse>> { IsError = false, Payload = new Pagination<UserModelResponse>() };
        _userServiceMock.Setup(x => x.GetUserPaginationAsync(searchTerm, 0, 10)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetUserPagination(searchTerm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.FileName).Returns("test.jpg");
        formFileMock.Setup(f => f.Length).Returns(1024);
        var requestModel = new CreateUserRequestModel
        {
            Name = "a",
            Email = "",
            PhoneNumber = "",
            Image = formFileMock.Object,
            DeliveryId = Guid.NewGuid(),
            PartnerId = Guid.NewGuid(),
            SupplierId = Guid.NewGuid(),
            RoleName = "Admin" // Hoặc giá trị tương ứng với vai trò bạn muốn gán
        };
        var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
        _userServiceMock.Setup(x => x.CreateUser(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.CreateUser(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestModel = new UpdateUserRequestModel();
        var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
        _userServiceMock.Setup(x => x.UpdateUser(userId, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.UpdateUser(userId, requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.FileName).Returns("test.jpg");
        formFileMock.Setup(f => f.Length).Returns(1024);
        var requestModel = new UpdateProfileModel("", "", formFileMock.Object, Guid.NewGuid());
        var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
        _userServiceMock.Setup(x => x.UpdateProfile(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.UpdateProfile(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateUserLoginGoogle_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestModel = new UpdateUserLoginGoogleRequest("", new Guid());
        var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
        _userServiceMock.Setup(x => x.UpdateUserLoginGoogle(userId, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.UpdateUserLoginGoogle(userId, requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdateImageUser_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var image = new FormFile(Stream.Null, 0, 0, "data", "test.jpg");
        var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
        _userServiceMock.Setup(x => x.UpdateImageUser(userId, image)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.UpdateImageUser(userId, image);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateImageUser_ReturnsInternalServerError_WhenServiceFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var image = new FormFile(Stream.Null, 0, 0, "data", "test.jpg");
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } } };
        _userServiceMock.Setup(x => x.UpdateImageUser(userId, image)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.UpdateImageUser(userId, image);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetDeliveryStaffByDeliveryAdmin_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var mockResponse = new OperationResult<dynamic> { IsError = false, Payload = new dynamic[] { } };
        _userServiceMock.Setup(x => x.GetDeliveryStaffByDeliveryAdmin(0, 10)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetDeliveryStaffByDeliveryAdmin();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetDeliveryStaffByDeliveryAdminList_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var mockResponse = new OperationResult<List<UserResponse>> { IsError = false, Payload = new List<UserResponse>() };
        _userServiceMock.Setup(x => x.GetDeliveryStaffByDeliveryAdminList()).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetDeliveryStaffByDeliveryAdminList();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task GetDeliveryStaffByDeliveryAdmin_ReturnsInternalServerError_WhenServiceFails()
    {
        // Arrange
        var mockResponse = new OperationResult<dynamic> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } } };
        _userServiceMock.Setup(x => x.GetDeliveryStaffByDeliveryAdmin(0, 10)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetDeliveryStaffByDeliveryAdmin();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetDeliveryStaffByDeliveryAdminList_ReturnsInternalServerError_WhenServiceFails()
    {
        // Arrange
        var mockResponse = new OperationResult<List<UserResponse>> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } } };
        _userServiceMock.Setup(x => x.GetDeliveryStaffByDeliveryAdminList()).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.GetDeliveryStaffByDeliveryAdminList();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }
    
    [Fact]
    public async Task ChangePassword_ReturnsInternalServerError_WhenServiceFails()
    {
        // Arrange
        var changePassword = new ChangePassword("", "", "");
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } } };
        _userServiceMock.Setup(x => x.ChangePassword(changePassword)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.ChangePassword(changePassword);

        // Assert
        var statusCode = (int)((ObjectResult)result).StatusCode;
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
    }

    [Fact]
    public async Task ChangePassword_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var changePassword = new ChangePassword("", "", "");
        var mockResponse = new OperationResult<bool> { IsError = false, Payload = true };
        _userServiceMock.Setup(x => x.ChangePassword(changePassword)).ReturnsAsync(mockResponse);

        // Act
        var result = await _userController.ChangePassword(changePassword);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

}