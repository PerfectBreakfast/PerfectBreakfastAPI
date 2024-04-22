using System.Linq.Expressions;
using FluentAssertions;
using Mapster;
using MockQueryable.Moq;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class SupplierServicesTests : SetupTest
{
    private readonly ISupplierService _supplierService;

    public SupplierServicesTests()
    {
        _supplierService = new SupplierService(_unitOfWorkMock.Object, _mapper, _claimServiceMock.Object);
    }

    [Fact]
    public async Task GetSuppliers_ReturnsAllSuppliers()
    {
        // Arrange
        var suppliers = new List<Supplier>
        {
            new Supplier
            {
                Id = Guid.NewGuid(),
                Name = "Supplier 1",
                IsDeleted = false,
                Address = "null",
                PhoneNumber = ""
            },
            new Supplier
            {
                Id = Guid.NewGuid(),
                Name = "Supplier 2",
                IsDeleted = false,
                Address = "",
                PhoneNumber = ""
            }
        };

        _unitOfWorkMock.Setup(x => x.SupplierRepository.FindAll(It.IsAny<Expression<Func<Supplier, bool>>>()))
            .Returns(suppliers.AsQueryable().BuildMock());
        /*_mapper.Setup(m => m.Map<List<SupplierResponse>>(suppliers))
            .Returns(suppliers.Select(s => new SupplierResponse(s.Id, s.Name)).ToList());*/

        // Act
        var result = await _supplierService.GetSuppliers();

        // Assert
        Assert.NotNull(result.Payload);
        Assert.Equal(2, result.Payload.Count);
        Assert.Equal(suppliers[0].Name, result.Payload[0].Name);
    }

    [Fact]
    public async Task GetSuppliers_HandlesException()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.SupplierRepository.FindAll(It.IsAny<Expression<Func<Supplier, bool>>>()))
            .Throws(new Exception("Database error"));

        // Act
        var result = await _supplierService.GetSuppliers();

        // Assert
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }

    [Fact]
    public async Task CreateSupplier_HandlesExceptions_WhenAnErrorOccurs()
    {
        // Arrange
        var requestModel = new CreateSupplierRequestModel
        {
            Name = "Error Supplier",
            Address = "Error Address",
            PhoneNumber = "Error Phone"
        };

        /*_mapper.Setup(m => m.Map<Supplier>(requestModel)).Returns(new Supplier());*/
        _unitOfWorkMock.Setup(u => u.SupplierRepository.AddAsync(It.IsAny<Supplier>()))
            .ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await _supplierService.CreateSupplier(requestModel);

        // Assert
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Message.Contains("An error occurred"));
    }
    
    [Fact]
    public async Task RemoveSupplier_Successful()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier
        {
            Id = supplierId,
            Name = "Test Supplier",
            IsDeleted = false,
            Address = "",
            PhoneNumber = ""
        };
        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetByIdAsync(supplierId)).ReturnsAsync(supplier);
        _unitOfWorkMock.Setup(u => u.SupplierRepository.SoftRemove(supplier));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _supplierService.RemoveSupplier(supplierId);

        // Assert
        Assert.NotNull(result.Payload);
        _unitOfWorkMock.Verify(u => u.SupplierRepository.SoftRemove(supplier), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once());
    }
    
    
    [Fact]
    public async Task RemoveSupplier_Exception_ReturnsUnknownError()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetByIdAsync(supplierId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _supplierService.RemoveSupplier(supplierId);

        // Assert
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task UpdateSupplier_Successful()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier
        {
            Id = supplierId,
            Name = "Original Name",
            Address = "Original Address",
            PhoneNumber = "Original Phone"
        };
        var requestModel = new UpdateSupplierRequestModel
        {
            Name = "Updated Name",
            Address = "Updated Address",
            PhoneNumber = "Updated Phone"
        };

        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetByIdAsync(supplierId)).ReturnsAsync(supplier);
        _unitOfWorkMock.Setup(u => u.SupplierRepository.Update(supplier));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
        //_mapper.Setup(m => m.Map<SupplierResponse>(supplier)).Returns(new SupplierResponse(supplier.Id, supplier.Name, supplier.Address, supplier.PhoneNumber, null, null, new List<string>(), new List<string?>(), 0));

        // Act
        var result = await _supplierService.UpdateSupplier(supplierId, requestModel);

        // Assert
        Assert.NotNull(result.Payload);
        Assert.Equal("Updated Name", result.Payload.Name);
        _unitOfWorkMock.Verify(u => u.SupplierRepository.Update(supplier), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once());
    }

    [Fact]
    public async Task UpdateSupplier_ThrowsException_ReturnsError()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var requestModel = new UpdateSupplierRequestModel { Name = "New Name" };
        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetByIdAsync(supplierId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _supplierService.UpdateSupplier(supplierId, requestModel);

        // Assert
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task GetSupplierByFood_ReturnsSuppliers_WhenSuppliersExistForFood()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);

        var user = new User { PartnerId = Guid.NewGuid() };
        var suppliers = new List<Supplier>
        {
            new Supplier
            {
                Id = Guid.NewGuid(),
                SupplierCommissionRates = new List<SupplierCommissionRate>
                {
                    new SupplierCommissionRate
                    {
                        FoodId = foodId
                    }
                },
                Name = "",
                Address = "",
                PhoneNumber = ""
            }
        };
        
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetSupplierByPartner(user.PartnerId.Value)).ReturnsAsync(suppliers);

        //_mapper.Setup(m => m.Map<List<SupplierDTO>>(It.IsAny<List<Supplier>>())).Returns(new List<SupplierDTO>());

        // Act
        var result = await _supplierService.GetSupplierByFood(foodId);

        // Assert
        //Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Payload);
    }
    
    [Fact]
    public async Task GetSupplierByFood_ReturnsError_WhenNoSuppliersRegisteredForFood()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);

        var user = new User { PartnerId = Guid.NewGuid() };
        var suppliers = new List<Supplier>
        {
            new Supplier
            {
                Id = Guid.NewGuid(),
                SupplierCommissionRates = new List<SupplierCommissionRate>(),
                Name = "",
                Address = "",
                PhoneNumber = ""
            }
        };
        
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetSupplierByPartner(user.PartnerId.Value)).ReturnsAsync(suppliers);

        // Act
        var result = await _supplierService.GetSupplierByFood(foodId);

        // Assert
        Assert.Contains(result.Errors, e => e.Code == ErrorCode.BadRequest);
    }
    
    [Fact]
    public async Task GetSupplierByFood_ThrowsException_ReturnsError()
    {
        // Arrange
        var foodId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);

        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _supplierService.GetSupplierByFood(foodId);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task GetAllSupplierByPartner_ReturnsSuppliers_WhenSuppliersExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var suppliers = new List<Supplier>
        {
            new Supplier
            {
                Id = Guid.NewGuid(),
                IsDeleted = false,
                Name = "null",
                Address = "",
                PhoneNumber = ""
            },
            new Supplier
            {
                Id = Guid.NewGuid(),
                IsDeleted = false,
                Name = "",
                Address = "",
                PhoneNumber = ""
            }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId))
            .ReturnsAsync(new User { PartnerId = partnerId });
        _unitOfWorkMock.Setup(u => u.SupplierRepository.GetSupplierByPartner(partnerId))
            .ReturnsAsync(suppliers);

        /*_mapper.Setup(m => m.Map<List<SupplierDTO>>(It.IsAny<List<Supplier>>()))
            .Returns(new List<SupplierDTO>()); // Adjust mapping as necessary*/

        // Act
        var result = await _supplierService.GetAllSupplierByPartner();

        // Assert
        Assert.NotEmpty(result.Payload);
    }
    

    [Fact]
    public async Task GetAllSupplierByPartner_ThrowsException_WhenUserRepositoryFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _supplierService.GetAllSupplierByPartner();

        // Assert
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Database error"));
    }
    
}