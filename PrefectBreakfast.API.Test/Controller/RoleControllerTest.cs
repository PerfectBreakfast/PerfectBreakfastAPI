using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class RoleControllerTest : SetupTest
{
    private readonly RoleController _roleController;

    public RoleControllerTest()
    {
        _roleController = new RoleController(_roleServiceMock.Object);
    }
    
    [Fact]
    public async Task GetManagementRole_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<RoleResponse>>
        {
            Payload = new List<RoleResponse> { new RoleResponse { Id = Guid.NewGuid(), Name = "Admin" } },
            IsError = false
        };
        _roleServiceMock.Setup(x => x.GetManagementRole()).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.GetManagementRole();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetManagementRole_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<RoleResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "No management roles found." } }
        };
        _roleServiceMock.Setup(x => x.GetManagementRole()).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.GetManagementRole();

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetRoleById_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid roleId = Guid.NewGuid();
        var mockResponse = new OperationResult<RoleResponse>
        {
            Payload = new RoleResponse { Id = roleId, Name = "User" },
            IsError = false
        };
        _roleServiceMock.Setup(x => x.GetRoleById(roleId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.GetRoleId(roleId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetRoleById_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid roleId = Guid.NewGuid();
        var mockResponse = new OperationResult<RoleResponse>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Role not found." } }
        };
        _roleServiceMock.Setup(x => x.GetRoleById(roleId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.GetRoleId(roleId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task GetRoleByUnitId_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid unitId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<RoleResponse>>
        {
            Payload = new List<RoleResponse> { new RoleResponse { Id = Guid.NewGuid(), Name = "Role1" } },
            IsError = false
        };
        _roleServiceMock.Setup(x => x.GetRoleByUnitId(unitId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.GetRoleByUnitId(unitId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task GetRoleByUnitId_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid unitId = Guid.NewGuid();
        var mockResponse = new OperationResult<List<RoleResponse>>
        {
            IsError = true,
            Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Roles not found for unit." } }
        };
        _roleServiceMock.Setup(x => x.GetRoleByUnitId(unitId)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.GetRoleByUnitId(unitId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateRole_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var requestModel = new CreatRoleRequest();
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _roleServiceMock.Setup(x => x.CreateRole(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.CreateRole(requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task CreateRole_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        var requestModel = new CreatRoleRequest { Name = "New Role" };
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.ServerError, Message = "Failed to create role." } } };
        _roleServiceMock.Setup(x => x.CreateRole(requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.CreateRole(requestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateRole_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var requestModel = new UpdateRolerequest { Name = "Updated Role" };
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _roleServiceMock.Setup(x => x.UpdateRole(id, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.UpdateRole(id, requestModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task UpdateRole_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var requestModel = new UpdateRolerequest { Name = "Updated Role" };
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Role not found." } } };
        _roleServiceMock.Setup(x => x.UpdateRole(id, requestModel)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.UpdateRole(id, requestModel);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task RemoveRole_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var mockResponse = new OperationResult<bool> { Payload = true, IsError = false };
        _roleServiceMock.Setup(x => x.RemoveRole(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.RemoveRole(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }

    [Fact]
    public async Task RemoveRole_ReturnsError_WhenServiceErrors()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var mockResponse = new OperationResult<bool> { IsError = true, Errors = new List<Error> { new Error { Code = ErrorCode.NotFound, Message = "Role not found to delete." } } };
        _roleServiceMock.Setup(x => x.RemoveRole(id)).ReturnsAsync(mockResponse);

        // Act
        var result = await _roleController.RemoveRole(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}