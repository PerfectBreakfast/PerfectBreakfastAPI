using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Request;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class SupplyAssigmentServiceTests : SetupTest
{
    private readonly ISupplyAssigmentService _supplyAssigmentService;

    public SupplyAssigmentServiceTests()
    {
        _supplyAssigmentService = new SupplyAssigmentService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetSupplyAssigment_ReturnsAllSupplyAssignments()
    {
        // Arrange
        var supplyAssignments = new List<SupplyAssignment>
        {
            new SupplyAssignment { SupplierId = Guid.NewGuid(), PartnerId = Guid.NewGuid() },
            new SupplyAssignment { SupplierId = Guid.NewGuid(), PartnerId = Guid.NewGuid() }
        };
        var expectedResponse = _mapper.Map<List<SupplyAssigmentResponse>>(supplyAssignments);
        _unitOfWorkMock.Setup(u => u.SupplyAssigmentRepository.GetAllAsync()).ReturnsAsync(supplyAssignments);

        // Act
        var result = await _supplyAssigmentService.GetSupplyAssigment();

        // Assert
        result.Payload.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task GetSupplyAssigment_HandlesExceptions()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.SupplyAssigmentRepository.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _supplyAssigmentService.GetSupplyAssigment();

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == "Database error");
    }
    
    [Fact]
    public async Task CreateSupplyAssigment_AddsNewAssignment_WhenNoDuplicateExists()
    {
        // Arrange
        var requestModel = new CreateSupplyAssigment
        {
            SupplierId = Guid.NewGuid(),
            PartnerId = Guid.NewGuid()
        };
        var supplyAssignment = _mapper.Map<SupplyAssignment>(requestModel);
        var expectedResponse = _mapper.Map<SupplyAssigmentResponse>(supplyAssignment);

        _unitOfWorkMock.Setup(u => u.SupplyAssigmentRepository.IsDuplicateAssignment(requestModel.PartnerId.Value, requestModel.SupplierId.Value))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.SupplyAssigmentRepository.AddAsync(It.IsAny<SupplyAssignment>()))
            .ReturnsAsync(supplyAssignment);
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _supplyAssigmentService.CreateSupplyAssigment(requestModel);

        // Assert
        result.Payload.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task CreateSupplyAssigment_HandlesExceptions_WhenDatabaseFails()
    {
        // Arrange
        var requestModel = new CreateSupplyAssigment
        {
            SupplierId = Guid.NewGuid(),
            PartnerId = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(u => u.SupplyAssigmentRepository.IsDuplicateAssignment(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.SupplyAssigmentRepository.AddAsync(It.IsAny<SupplyAssignment>()))
            .ThrowsAsync(new Exception("Database insert failed"));

        // Act
        var result = await _supplyAssigmentService.CreateSupplyAssigment(requestModel);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == "Database insert failed");
    }
    
    [Fact]
    public async Task RemoveSupplyAssigment_RemovesAssignment_IfFound()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var supplyAssignment = new SupplyAssignment { SupplierId = supplierId, PartnerId = partnerId };

        _unitOfWorkMock.Setup(u => u.SupplyAssigmentRepository.FindSingleAsync(It.IsAny<Expression<Func<SupplyAssignment, bool>>>()))
            .ReturnsAsync(supplyAssignment);
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _supplyAssigmentService.RemoveSupplyAssigment(supplierId, partnerId);

        // Assert
        _unitOfWorkMock.Verify(u => u.SupplyAssigmentRepository.Remove(supplyAssignment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task RemoveSupplyAssigment_HandlesExceptions_WhenEntityNotFound()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.SupplyAssigmentRepository.FindSingleAsync(It.IsAny<Expression<Func<SupplyAssignment, bool>>>()))
            .ReturnsAsync(value: null);  // No entity found

        // Act
        var result = await _supplyAssigmentService.RemoveSupplyAssigment(supplierId, partnerId);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == "Id not found!");
    }
}