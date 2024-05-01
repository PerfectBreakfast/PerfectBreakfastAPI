using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Contracts.Commons;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Commons;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class BaseControllerTests : SetupTest
{
    [Fact]
    public void HandleErrorResponse_UnauthorizedError_ReturnsUnauthorizedResult()
    {
        // Arrange
        var controller = new BaseController();
        var errors = new List<Error> { new Error { Code = ErrorCode.UnAuthorize, Message = "Unauthorized access" } };

        // Act
        var result = controller.HandleErrorResponse(errors);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(unauthorizedResult.Value);
        Assert.Equal(401, unauthorizedResult.StatusCode);
        Assert.Equal("Unauthorized access", response.Errors);
    }

    [Fact]
    public void HandleErrorResponse_NotFoundError_ReturnsNotFoundResult()
    {
        // Arrange
        var controller = new BaseController();
        var errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Item not found" } };

        // Act
        var result = controller.HandleErrorResponse(errors);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Equal("Item not found", response.Errors);
    }

    [Fact]
    public void HandleErrorResponse_ServerError_ReturnsServerErrorResult()
    {
        // Arrange
        var controller = new BaseController();
        var errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Internal server error" } };

        // Act
        var result = controller.HandleErrorResponse(errors);

        // Assert
        var serverErrorResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(serverErrorResult.Value);
        Assert.Equal(500, serverErrorResult.StatusCode);
        Assert.Equal("Internal server error", response.Errors);
    }

    /*[Fact]
    public void HandleErrorResponse_OtherErrors_ReturnsBadRequestResult()
    {
        // Arrange
        var controller = new BaseController();
        var errors = new List<Error> { new Error { Code = ErrorCode.Other, Message = "Other error" } };

        // Act
        var result = controller.HandleErrorResponse(errors);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Other error", response.Errors);
    }*/
}