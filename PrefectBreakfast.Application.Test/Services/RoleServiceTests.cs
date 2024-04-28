using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class RoleServiceTests : SetupTest
{
    private readonly IRoleService _roleService;

    public RoleServiceTests()
    {
        _roleService = new RoleService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetManagementRole_ReturnsSuccess_WithRoles()
    {
        // Arrange
        var mockRoles = new List<Role>()
        {
            new Role { Name = "Admin" },
            new Role { Name = "Manager" },
            new Role { Name = ConstantRole.CUSTOMER } // Excluded customer role
        };

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetAllAsync()).ReturnsAsync(mockRoles);
        /*_mapperMock.Setup(m => m.Map<List<RoleResponse>>(It.IsAny<List<Role>>()))
            .ReturnsAsync(new List<RoleResponse>());*/

        // Act
        var result = await _roleService.GetManagementRole();

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNullOrEmpty();
        result.Payload.Count.Should().Be(2); // Excludes customer role
    }

    [Fact]
    public async Task GetManagementRole_ReturnsSuccess_WithoutRoles()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.RoleRepository.GetAllAsync()).ReturnsAsync(new List<Role>());
        /*_mapperMock.Setup(m => m.Map<List<RoleResponse>>(It.IsAny<List<Role>>()))
            .ReturnsAsync(new List<RoleResponse>());*/

        // Act
        var result = await _roleService.GetManagementRole();

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeEmpty();
    }

    [Fact]
    public async Task GetManagementRole_ReturnsError_OnException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.RoleRepository.GetAllAsync()).ThrowsAsync(new Exception("Error getting roles"));

        // Act
        var result = await _roleService.GetManagementRole();

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError);
    }

    [Fact]
    public async Task GetRoleById_ReturnsSuccess_WithRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var mockRole = new Role { Id = roleId, Name = "Admin" };

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ReturnsAsync(mockRole);
        //_mapperMock.Setup(m => m.Map<RoleResponse>(It.IsAny<Role>())).ReturnsAsync(new RoleResponse());

        // Act
        var result = await _roleService.GetRoleById(roleId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Id.Should().Be(roleId);
    }

    [Fact]
    public async Task GetRoleById_ReturnsNotFound_OnMissingRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _roleService.GetRoleById(roleId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound);
    }

    [Fact]
    public async Task GetRoleById_ReturnsError_OnException()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId))
            .ThrowsAsync(new Exception("Error getting role"));

        // Act
        var result = await _roleService.GetRoleById(roleId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError);
    }

    [Fact]
    public async Task GetRoleByUnitId_ReturnsSuccess_ForDeliveryUnit()
    {
        // Arrange
        var unitId = Guid.NewGuid();
        var mockUnit = new Delivery
        {
            Id = unitId,
            Name = "null",
            Address = "null",
            PhoneNumber = "null",
            CommissionRate = 0
        };

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetPartnerById(unitId)).ReturnsAsync((Partner?)null);
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetDeliveryById(unitId)).ReturnsAsync(mockUnit);
        _unitOfWorkMock.Setup(u => u.RoleRepository.FindAll(x => x.UnitCode == UnitCode.Delivery))
            .Returns(new List<Role>().AsQueryable().BuildMock());
        /*_mapperMock.Setup(m => m.Map<List<RoleResponse>>(It.IsAny<List<Role>>()))
            .ReturnsAsync(new List<RoleResponse>());*/

        // Act
        var result = await _roleService.GetRoleByUnitId(unitId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task GetRoleByUnitId_ReturnsSuccess_ForSupplier()
    {
        // Arrange
        var unitId = Guid.NewGuid();
        var mockUnit = new Supplier
        {
            Id = unitId,
            Name = "null",
            Address = "null",
            PhoneNumber = "null"
        };

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetPartnerById(unitId)).ReturnsAsync((Partner?)null);
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetDeliveryById(unitId)).ReturnsAsync((Delivery?)null);
        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetSupplierById(unitId)).ReturnsAsync(mockUnit);
        _unitOfWorkMock.Setup(u => u.RoleRepository.FindAll(x => x.UnitCode == UnitCode.Supplier))
            .Returns(new List<Role>().AsQueryable().BuildMock);
        /*_mapperMock.Setup(m => m.Map<List<RoleResponse>>(It.IsAny<List<Role>>()))
            .ReturnsAsync(new List<RoleResponse>());*/

        // Act
        var result = await _roleService.GetRoleByUnitId(unitId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task GetRoleByUnitId_ReturnsNotFound_OnMissingUnit()
    {
        // Arrange
        var unitId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetPartnerById(unitId)).ReturnsAsync((Partner?)null);
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetDeliveryById(unitId)).ReturnsAsync((Delivery?)null);
        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetSupplierById(unitId)).ReturnsAsync((Supplier?)null);

        // Act
        var result = await _roleService.GetRoleByUnitId(unitId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound);
    }

    [Fact]
    public async Task GetRoleByUnitId_ReturnsError_OnException()
    {
        // Arrange
        var unitId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetPartnerById(unitId))
            .ThrowsAsync(new Exception("Error getting unit"));

        // Act
        var result = await _roleService.GetRoleByUnitId(unitId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError);
    }

    /*[Fact]
    public async Task CreateRole_ReturnsSuccess_WithRole()
    {
        // Arrange
        var requestModel = new CreatRoleRequest { Name = "Shipper" };

        _unitOfWorkMock.Setup(u => u.RoleManager.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _roleService.CreateRole(requestModel);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeTrue();
    }*/

    /*[Fact]
    public async Task CreateRole_ReturnsError_OnException()
    {
        // Arrange
        var requestModel = new CreatRoleRequest { Name = "Shipper" };

        _unitOfWorkMock.Setup(u => u.RoleManager.CreateAsync(It.IsAny<Role>()))
            .ThrowsAsync(new Exception("Error creating role"));

        // Act
        var result = await _roleService.CreateRole(requestModel);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError);
    }*/

    [Fact]
    public async Task UpdateRole_ReturnsSuccess_WithRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var requestModel = new UpdateRolerequest { Name = "Shipper Updated" };
        var mockRole = new Role { Id = roleId, Name = "Shipper" };

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ReturnsAsync(mockRole);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1); // Simulate save success

        // Act
        var result = await _roleService.UpdateRole(roleId, requestModel);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRole_ReturnsNotFound_OnMissingRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var requestModel = new UpdateRolerequest { Name = "Shipper Updated" };

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _roleService.UpdateRole(roleId, requestModel);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound);
    }

    [Fact]
    public async Task UpdateRole_ReturnsError_OnSaveChangeFails()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var requestModel = new UpdateRolerequest { Name = "Shipper Updated" };
        var mockRole = new Role { Id = roleId, Name = "Shipper" };

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ReturnsAsync(mockRole);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(0); // Simulate save failure

        // Act
        var result = await _roleService.UpdateRole(roleId, requestModel);

        // Assert
        result.Payload.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateRole_ReturnsError_OnException()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var requestModel = new UpdateRolerequest { Name = "Shipper Updated" };

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId))
            .ThrowsAsync(new Exception("Error updating role"));

        // Act
        var result = await _roleService.UpdateRole(roleId, requestModel);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError);
    }
    
    [Fact]
    public async Task RemoveRole_ReturnsSuccess_WithRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var mockRole = new Role { Id = roleId, Name = "Shipper" };

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ReturnsAsync(mockRole);
        _unitOfWorkMock.Setup(u => u.RoleRepository.Remove(mockRole)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1); // Simulate save success

        // Act
        var result = await _roleService.RemoveRole(roleId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.RoleRepository.Remove(mockRole), Times.Once);
    }
    
    [Fact]
    public async Task RemoveRole_ReturnsNotFound_OnMissingRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _roleService.RemoveRole(roleId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound);
    }
    
    [Fact]
    public async Task RemoveRole_ReturnsError_OnSaveChangeFails()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var mockRole = new Role { Id = roleId, Name = "Shipper" };

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ReturnsAsync(mockRole);
        _unitOfWorkMock.Setup(u => u.RoleRepository.Remove(mockRole)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(0); // Simulate save failure

        // Act
        var result = await _roleService.RemoveRole(roleId);

        // Assert
        result.Payload.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.RoleRepository.Remove(mockRole), Times.Once);
    }
    
    [Fact]
    public async Task RemoveRole_ReturnsError_OnException()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.RoleRepository.GetByIdAsync(roleId)).ThrowsAsync(new Exception("Error removing role"));

        // Act
        var result = await _roleService.RemoveRole(roleId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError);
    }
}