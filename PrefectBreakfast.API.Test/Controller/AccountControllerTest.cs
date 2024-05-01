using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.AuthModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class AccountControllerTest : SetupTest
{
    private readonly AccountController _accountController;

    public AccountControllerTest()
    {
        _accountController = new AccountController(_userServiceMock.Object);
    }
    
    [Fact]
    public async Task SignUp_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var request = new SignUpModel();
        var mockResponse = new OperationResult<bool>
        {
            Payload = true,
            IsError = false
        };
        _userServiceMock.Setup(x => x.SignUp(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.SignUp(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task SignUp_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var request = new SignUpModel();
        var mockResponse = new OperationResult<bool>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid data" } }
        };
        _userServiceMock.Setup(x => x.SignUp(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.SignUp(request);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task SignInForCustomer_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var request = new SignInModel();
        var roles = new List<string> { "Admin", "User" };
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            Payload = new UserLoginResponse(Guid.NewGuid(), "", "", "", roles, "", ""),
            IsError = false
        };
        _userServiceMock.Setup(x => x.SignIn(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.SignInForCustomer(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task SignInForCustomer_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var request = new SignInModel();
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.UnAuthorize, Message = "Unauthorized" } }
        };
        _userServiceMock.Setup(x => x.SignIn(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.SignInForCustomer(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
    
    [Fact]
    public async Task GoogleLogin_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var code = "sampleCode";
        var roles = new List<string> { "Admin", "User" };
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            Payload = new UserLoginResponse(Guid.NewGuid(), "", "", "", roles, "", ""),
            IsError = false
        };
        _userServiceMock.Setup(x => x.ExternalLogin(code)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.GoogleLogin(code);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GoogleLogin_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var code = "sampleCode";
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.UnAuthorize, Message = "Login failed" } }
        };
        _userServiceMock.Setup(x => x.ExternalLogin(code)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.GoogleLogin(code);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
    
    [Fact]
    public async Task ManagementLogin_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var request = new ManagementLoginModel("email", "password", Guid.NewGuid());  // Assume necessary details are filled
        var roles = new List<string> { "Admin", "User" };
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            Payload = new UserLoginResponse(Guid.NewGuid(), "", "", "", roles, "", ""),
            IsError = false
        };
        _userServiceMock.Setup(x => x.ManagementLogin(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.ManagementLogin(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task ManagementLogin_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var request = new ManagementLoginModel("email", "password", Guid.NewGuid());
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.UnAuthorize, Message = "Unauthorized" } }
        };
        _userServiceMock.Setup(x => x.ManagementLogin(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.ManagementLogin(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
    
    [Fact]
    public async Task RefreshUserToken_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var tokenModel = new TokenModel("", "");  // Assume necessary token information
        var roles = new List<string> { "Admin", "User" };
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            Payload = new UserLoginResponse(Guid.NewGuid(), "", "", "", roles, "", ""),
            IsError = false
        };
        _userServiceMock.Setup(x => x.RefreshUserToken(tokenModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.RefreshUserToken(tokenModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task RefreshUserToken_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var tokenModel = new TokenModel("", "");
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid token" } }
        };
        _userServiceMock.Setup(x => x.RefreshUserToken(tokenModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.RefreshUserToken(tokenModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task GetCurrentUser_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<UserDetailResponse>
        {
            Payload = new UserDetailResponse(Guid.NewGuid(), "", "", "", "", "", true, true, Guid.NewGuid(), "", new List<string>()),
            IsError = false
        };
        _userServiceMock.Setup(x => x.GetCurrentUser()).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.GetCurrentUser();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<UserDetailResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.UnAuthorize, Message = "Unauthorized" } }
        };
        _userServiceMock.Setup(x => x.GetCurrentUser()).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.GetCurrentUser();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task GeneratePasswordResetToken_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var email = "test@example.com";
        var mockResponse = new OperationResult<string>
        {
            Payload = "resetTokenString",
            IsError = false
        };
        _userServiceMock.Setup(x => x.GeneratePasswordResetToken(email)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.GeneratePasswordResetToken(email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GeneratePasswordResetToken_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var email = "test@example.com";
        var mockResponse = new OperationResult<string>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid email" } }
        };
        _userServiceMock.Setup(x => x.GeneratePasswordResetToken(email)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.GeneratePasswordResetToken(email);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task ResetPassword_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var request = new ResetPasswordRequest("", "", "");  // Assume necessary details are filled
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            Payload = new UserLoginResponse(Guid.NewGuid(), "", "", "", new List<string?>(), "", ""),
            IsError = false
        };
        _userServiceMock.Setup(x => x.ResetPassword(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.ResetPassword(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task ResetPassword_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var request = new ResetPasswordRequest("", "", "");
        var mockResponse = new OperationResult<UserLoginResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.BadRequest, Message = "Invalid request" } }
        };
        _userServiceMock.Setup(x => x.ResetPassword(request)).ReturnsAsync(mockResponse);

        // Act
        var result = await _accountController.ResetPassword(request);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

}