using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class SupplierCommissionRateServiceTests : SetupTest
{
    private readonly ISupplierCommissionRateService _supplierCommissionRateService;
    public SupplierCommissionRateServiceTests()
    {
        _supplierCommissionRateService = new SupplierCommissionRateService(_unitOfWorkMock.Object, _mapper);
    }
    
    [Fact]
    public async Task GetSupplierCommissionRateId_ReturnsData_WhenIdExists()
    {
        // Arrange
        var commissionRateId = Guid.NewGuid();
        var supplierCommissionRate = new SupplierCommissionRate
        {
            Id = commissionRateId,
            CommissionRate = 10,
            Food = new Food { Id = Guid.NewGuid(), Name = "Apple" },
            Supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Name = "Best Supplier",
                Address = string.Empty,
                PhoneNumber = string.Empty
            }
        };

        _unitOfWorkMock.Setup(u => u.SupplierCommissionRateRepository.FindSingleAsync(It.IsAny<Expression<Func<SupplierCommissionRate, bool>>>(), It.IsAny<Expression<Func<SupplierCommissionRate, object>>>(), It.IsAny<Expression<Func<SupplierCommissionRate, object>>>()))
            .ReturnsAsync(supplierCommissionRate);

        /*_mapperMock.Setup(m => m.Map<SupplierCommissionRateRespone>(It.IsAny<SupplierCommissionRate>()))
            .Returns(new SupplierCommissionRateRespone { Id = commissionRateId, CommissionRate = 10 });*/
        _mapper.Map<SupplierCommissionRateRespone>(supplierCommissionRate);

        // Act
        var result = await _supplierCommissionRateService.GetSupplierCommissionRateId(commissionRateId);

        // Assert
        Assert.NotNull(result.Payload);
        Assert.Equal(commissionRateId, result.Payload.Id);
        _unitOfWorkMock.Verify(u => u.SupplierCommissionRateRepository.FindSingleAsync(It.IsAny<Expression<Func<SupplierCommissionRate, bool>>>(), It.IsAny<Expression<Func<SupplierCommissionRate, object>>>(), It.IsAny<Expression<Func<SupplierCommissionRate, object>>>()), Times.Once);
    }
    
    [Fact]
    public async Task GetSupplierCommissionRateId_ReturnsError_WhenIdDoesNotExist()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.SupplierCommissionRateRepository.FindSingleAsync(It.IsAny<Expression<Func<SupplierCommissionRate, bool>>>(), It.IsAny<Expression<Func<SupplierCommissionRate, object>>>(), It.IsAny<Expression<Func<SupplierCommissionRate, object>>>()))
            .ReturnsAsync((SupplierCommissionRate)null);

        // Act
        var result = await _supplierCommissionRateService.GetSupplierCommissionRateId(Guid.NewGuid());

        // Assert
        Assert.Null(result.Payload);
        Assert.Equal("Id is not exsit", result.Errors.First().Message);
    }
    
    [Fact]
    public async Task GetSupplierCommissionRateId_ReturnsError_OnException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.SupplierCommissionRateRepository.FindSingleAsync(It.IsAny<Expression<Func<SupplierCommissionRate, bool>>>(), It.IsAny<Expression<Func<SupplierCommissionRate, object>>>(), It.IsAny<Expression<Func<SupplierCommissionRate, object>>>()))
            .ThrowsAsync(new Exception("Error"));

        // Act
        var result = await _supplierCommissionRateService.GetSupplierCommissionRateId(Guid.NewGuid());

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Error"));
    }
    
    [Fact]
    public async Task GetSupplierMoreFood_ReturnsFoodList_WhenSupplierExists()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var foods = new List<Food>
        {
            new Food { Id = Guid.NewGuid(), Name = "Apple" },
            new Food { Id = Guid.NewGuid(), Name = "Banana" }
        };

        var supplierCommissions = foods.Select(food => new SupplierCommissionRate { Food = food }).ToList();

        _unitOfWorkMock.Setup(u => u.SupplierCommissionRateRepository.GetBySupplierId(supplierId))
            .ReturnsAsync(supplierCommissions);

        /*_mapperMock.Setup(m => m.Map<List<FoodResponse>>(It.IsAny<List<Food>>()))
            .Returns(foods.Select(f => new FoodResponse { Id = f.Id, Name = f.Name }).ToList());*/

        // Act
        var result = await _supplierCommissionRateService.GetSupplierMoreFood(supplierId);

        // Assert
        Assert.NotNull(result.Payload);
        Assert.Equal(2, result.Payload.Count);
        Assert.Contains(result.Payload, f => f.Name == "Apple");
        Assert.Contains(result.Payload, f => f.Name == "Banana");
    }
    
    [Fact]
    public async Task GetSupplierMoreFood_ReturnsEmpty_WhenNoFoodsFound()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.SupplierCommissionRateRepository.GetBySupplierId(supplierId))
            .ReturnsAsync(new List<SupplierCommissionRate>());

        // Act
        var result = await _supplierCommissionRateService.GetSupplierMoreFood(supplierId);

        // Assert
        Assert.Empty(result.Payload);
    }
    
    [Fact]
    public async Task GetSupplierMoreFood_ReturnsError_OnException()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.SupplierCommissionRateRepository.GetBySupplierId(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _supplierCommissionRateService.GetSupplierMoreFood(supplierId);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Database failure"));
    }
    
    [Fact]
    public async Task Delete_ReturnsSuccess_WhenIdExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var commissionRate = new SupplierCommissionRate { Id = id };

        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.GetByIdAsync(id))
            .ReturnsAsync(commissionRate);

        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.Remove(commissionRate));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _supplierCommissionRateService.Delete(id);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once());
    }
    
    [Fact]
    public async Task Delete_ReturnsError_WhenIdDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.GetByIdAsync(id))
            .ThrowsAsync(new NotFoundIdException()); // Simulate no commission rate found

        // Act
        var result = await _supplierCommissionRateService.Delete(id);

        // Assert
        result.Payload.Should().BeNull();
       
    }
    
    [Fact]
    public async Task Delete_ThrowsException_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var id = Guid.NewGuid();
        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.GetByIdAsync(id))
            .ThrowsAsync(new Exception("Database failure"));

        // Act
        var result = await _supplierCommissionRateService.Delete(id);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Database failure"));
    }
    
    [Fact]
    public async Task UpdateSupplierCommissionRate_SuccessfullyUpdates_WhenValidIdAndRateProvided()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingRate = new SupplierCommissionRate { Id = id, CommissionRate = 5 };
        var request = new UpdateSupplierCommissionRateRequest(10);

        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.GetByIdAsync(id))
            .ReturnsAsync(existingRate);
        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.Update(It.IsAny<SupplierCommissionRate>()));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _supplierCommissionRateService.UpdateSupplierCommissionRate(id, request);

        // Assert
        Assert.Equal(10, existingRate.CommissionRate);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateSupplierCommissionRate_ReturnsError_WhenIdDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateSupplierCommissionRateRequest(10);

        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.GetByIdAsync(id))
            .ThrowsAsync(new NotFoundIdException());  // Simulate no commission rate found

        // Act
        var result = await _supplierCommissionRateService.UpdateSupplierCommissionRate(id, request);

        // Assert
        Assert.Contains("Id is not exist", result.Errors.Select(e => e.Message));
    }
    
    
    [Fact]
    public async Task UpdateSupplierCommissionRate_CatchesException_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateSupplierCommissionRateRequest(10);

        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.GetByIdAsync(id))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _supplierCommissionRateService.UpdateSupplierCommissionRate(id, request);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task CreateSupplierCommissionRate_CreatesRatesSuccessfully_WhenValidRequests()
    {
        // Arrange
        var request = new CreateSupplierCommissionRateRequest
        {
            FoodIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            SupplierId = Guid.NewGuid(),
            CommissionRate = 10
        };

        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.AnyAsync(It.IsAny<Expression<Func<SupplierCommissionRate, bool>>>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.AddAsync(It.IsAny<SupplierCommissionRate>()));
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _supplierCommissionRateService.CreateSupplierCommissionRate(request);

        // Assert
        Assert.Equal(request.FoodIds.Count, result.Payload.Count);
        _unitOfWorkMock.Verify(x => x.SupplierCommissionRateRepository.AddAsync(It.IsAny<SupplierCommissionRate>()), Times.Exactly(request.FoodIds.Count));
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once());
    }
    
    [Fact]
    public async Task CreateSupplierCommissionRate_ReturnsError_WhenFoodIdMatchesSupplierId()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var request = new CreateSupplierCommissionRateRequest
        {
            FoodIds = new List<Guid> { supplierId },
            SupplierId = supplierId,
            CommissionRate = 10
        };

        // Act
        var result = await _supplierCommissionRateService.CreateSupplierCommissionRate(request);

        // Assert
        Assert.Contains(result.Errors, e => e.Message.Contains("Food ID and Supplier ID cannot be the same"));
    }
    
    [Fact]
    public async Task CreateSupplierCommissionRate_ReturnsError_WhenCommissionRateAlreadyExists()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var request = new CreateSupplierCommissionRateRequest
        {
            FoodIds = new List<Guid> { foodId },
            SupplierId = supplierId,
            CommissionRate = 10
        };

        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.AnyAsync(It.IsAny<Expression<Func<SupplierCommissionRate, bool>>>()))
            .ReturnsAsync(true); // Simulate existing commission rate

        // Act
        var result = await _supplierCommissionRateService.CreateSupplierCommissionRate(request);

        // Assert
        Assert.Contains(result.Errors, e => e.Message.Contains("already exists"));
    }
    
}