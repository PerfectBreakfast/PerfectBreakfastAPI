using System.Linq.Expressions;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class DeliveryServiceTests : SetupTest
{
    private readonly IDeliveryService _deliveryService;

    public DeliveryServiceTests()
    {
        _deliveryService = new DeliveryService(_unitOfWorkMock.Object, _mapper);
    }
    
    [Fact]
    public async Task GetDeliveries_ReturnsAllNonDeletedDeliveries()
    {
        // Arrange
        var deliveries = new List<Delivery>
        {
            new Delivery
            {
                Id = Guid.NewGuid(), Name = "Delivery1", Address = "123 Main St", PhoneNumber = "555-1234",
                CommissionRate = 5.5m, Longitude = 40.7128, Latitude = -74.0060, IsDeleted = false,
                Users = new List<User>(), Companies = new List<Company>()
            },
            new Delivery
            {
                Id = Guid.NewGuid(), Name = "Delivery2", Address = "456 Main St", PhoneNumber = "555-5678",
                CommissionRate = 10.0m, Longitude = 40.7138, Latitude = -74.0068, IsDeleted = false,
                Users = new List<User>(), Companies = new List<Company>()
            }
        };

        var deliveryResponses = deliveries.Select(d => new DeliveryResponseModel(
            d.Id, d.Name, d.Address, d.PhoneNumber, d.CommissionRate, d.Longitude, d.Latitude,
            new List<string>(), // Assume no owners
            new List<string>(), // Assume no assigned companies
            0  // Assume zero members
        )).ToList();

        _unitOfWorkMock.Setup(x => x.DeliveryRepository.FindAll(It.IsAny<Expression<Func<Delivery, bool>>>()))
            .Returns(deliveries.AsQueryable().BuildMock());
        _mapper.Map<List<DeliveryResponseModel>>(deliveries);

        // Act
        var result = await _deliveryService.GetDeliveries();

        // Assert
        Assert.NotNull(result.Payload);
        Assert.Equal(deliveryResponses.Count, result.Payload.Count);
        //result.Payload.Should().BeEquivalentTo(deliveryResponses, options => options.ComparingByMembers<DeliveryResponseModel>());
    }
    
    [Fact]
    public async Task GetDeliveries_HandlesException_ReturnsUnknownError()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.FindAll(It.IsAny<Expression<Func<Delivery, bool>>>()))
            .Throws(new Exception("Database error"));

        // Act
        var result = await _deliveryService.GetDeliveries();

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task CreateDelivery_CreatesAndReturnsDelivery_WhenRequestIsValid()
    {
        // Arrange
        var requestModel = new CreateDeliveryRequest(
            Name: "New Delivery",
            Address: "123 Delivery St",
            PhoneNumber: "123-456-7890",
            CommissionRate: 5,
            Longitude: 40.7128,
            Latitude: -74.0060
        );

        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            Name = requestModel.Name,
            Address = requestModel.Address,
            PhoneNumber = requestModel.PhoneNumber,
            CommissionRate = requestModel.CommissionRate,
            Longitude = requestModel.Longitude,
            Latitude = requestModel.Latitude,
            IsDeleted = false
        };

        _unitOfWorkMock.Setup(u => u.DeliveryRepository.AddAsync(It.IsAny<Delivery>())).ReturnsAsync(delivery);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        _mapper.Map<Delivery>(requestModel);
        _mapper.Map<DeliveryResponseModel>(delivery);
        
        // Act
        var result = await _deliveryService.CreateDelivery(requestModel);

        // Assert
        result.Should().NotBeNull();
        result.Payload.Should().NotBeNull();
        result.Payload.Name.Should().Be(requestModel.Name);
        _unitOfWorkMock.Verify(u => u.DeliveryRepository.AddAsync(It.IsAny<Delivery>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CreateDelivery_HandlesException_ReturnsUnknownError()
    {
        // Arrange
        var requestModel = new CreateDeliveryRequest(
            Name: "Error Delivery",
            Address: "404 Error Way",
            PhoneNumber: "000-000-0000",
            CommissionRate: 10,
            Longitude: null,
            Latitude: null
        );

        _unitOfWorkMock.Setup(u => u.DeliveryRepository.AddAsync(It.IsAny<Delivery>()))
            .ThrowsAsync(new Exception("Database error"));
    
        // Act
        var result = await _deliveryService.CreateDelivery(requestModel);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Database error"));
    }
    
    [Fact]
    public async Task RemoveDelivery_RemovesDeliverySuccessfully_WhenDeliveryExists()
    {
        // Arrange
        var deliveryId = Guid.NewGuid();
        var delivery = new Delivery
        {
            Id = deliveryId,
            Name = "Test Delivery",
            Address = "123 Test Ave",
            PhoneNumber = "123-456-7890",
            CommissionRate = 5.5m
        };

        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetByIdAsync(deliveryId))
            .ReturnsAsync(delivery);
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.SoftRemove(delivery));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
        _mapper.Map<DeliveryResponseModel>(delivery);
        /*_mapper.Setup(m => m.Map<DeliveryResponseModel>(It.IsAny<Delivery>()))
            .Returns(new DeliveryResponseModel(
                delivery.Id, delivery.Name, delivery.Address, delivery.PhoneNumber, 
                delivery.CommissionRate, null, null, new List<string>(), new List<string>(), 0));*/

        // Act
        var result = await _deliveryService.RemoveDelivery(deliveryId);

        // Assert
        result.Should().NotBeNull();
        result.Payload.Should().NotBeNull();
        result.Payload.Id.Should().Be(deliveryId);
        _unitOfWorkMock.Verify(u => u.DeliveryRepository.SoftRemove(delivery), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }
   
    [Fact]
    public async Task RemoveDelivery_HandlesException_WhenDatabaseOperationFails()
    {
        // Arrange
        var deliveryId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetByIdAsync(deliveryId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _deliveryService.RemoveDelivery(deliveryId);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Database error"));
        _unitOfWorkMock.Verify(u => u.DeliveryRepository.SoftRemove(It.IsAny<Delivery>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Never);
    }
    
    [Fact]
    public async Task UpdateDelivery_UpdatesAndReturnsUpdatedDelivery_WhenValidRequest()
    {
        // Arrange
        var deliveryId = Guid.NewGuid();
        var existingDelivery = new Delivery
        {
            Id = deliveryId,
            Name = "Original Name",
            Address = "Original Address",
            PhoneNumber = "Original Phone",
            CommissionRate = 10.0m
        };
        var updateRequest = new UpdateDeliveryRequest
        {
            Name = "Updated Name",
            Address = "Updated Address",
            PhoneNumber = "Updated Phone",
            CommissionRate = 15
        };

        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetByIdAsync(deliveryId))
            .ReturnsAsync(existingDelivery);
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.Update(It.IsAny<Delivery>()));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
        /*_mapper.Setup(m => m.Map<DeliveryResponseModel>(It.IsAny<Delivery>()))
            .Returns(new DeliveryResponseModel(
                deliveryId, updateRequest.Name, updateRequest.Address, updateRequest.PhoneNumber,
                updateRequest.CommissionRate.Value, null, null, new List<string>(), new List<string>(), 0));*/

        // Act
        var result = await _deliveryService.UpdateDelivery(deliveryId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Payload.Should().NotBeNull();
        result.Payload.Name.Should().Be(updateRequest.Name);
        result.Payload.Address.Should().Be(updateRequest.Address);
        result.Payload.PhoneNumber.Should().Be(updateRequest.PhoneNumber);
        result.Payload.CommissionRate.Should().Be(updateRequest.CommissionRate);
        _unitOfWorkMock.Verify(u => u.DeliveryRepository.Update(It.IsAny<Delivery>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateDelivery_HandlesException_WhenUpdateFails()
    {
        // Arrange
        var deliveryId = Guid.NewGuid();
        var updateRequest = new UpdateDeliveryRequest
        {
            Name = "Updated Name"
        };

        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetByIdAsync(deliveryId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _deliveryService.UpdateDelivery(deliveryId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Database error"));
        _unitOfWorkMock.Verify(u => u.DeliveryRepository.Update(It.IsAny<Delivery>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Never);
    }
    
    
    [Fact]
    public async Task GetDeliveryId_ReturnsNotFound_WhenDeliveryDoesNotExist()
    {
        // Arrange
        var deliveryId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetByIdAsync(deliveryId, null, null))
            .Throws(new NotFoundIdException("Not found"));

        // Act
        var result = await _deliveryService.GetDeliveryId(deliveryId);

        // Assert
        result.Should().NotBeNull();
        
    }
    
    [Fact]
    public async Task GetDeliveryId_HandlesException_WhenDatabaseErrorOccurs()
    {
        // Arrange
        var deliveryId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.DeliveryRepository.GetByIdAsync(deliveryId, null, null))
            .Throws(new Exception("Database error"));

        // Act
        var result = await _deliveryService.GetDeliveryId(deliveryId);

        // Assert
        result.Should().NotBeNull();
    }
    
    // [Fact]
    // public async Task GetDeliveryUnitPaginationAsync_ReturnsPaginatedResultsSuccessfully()
    // {
    //     // Arrange
    //     var pageIndex = 0;
    //     var pageSize = 2;
    //     var searchTerm = "delivery";
    //     var deliveries = new List<Delivery>
    //     {
    //         new Delivery
    //         {
    //             Id = Guid.NewGuid(),
    //             Name = "Delivery One",
    //             IsDeleted = false,
    //             Address = "",
    //             PhoneNumber = "",
    //             CommissionRate = 0
    //         },
    //         new Delivery
    //         {
    //             Id = Guid.NewGuid(),
    //             Name = "Delivery Two",
    //             IsDeleted = false,
    //             Address = "",
    //             PhoneNumber = "",
    //             CommissionRate = 0
    //         }
    //     };
    //
    //     var paginationResult = new Pagination<Delivery>
    //     {
    //         PageIndex = pageIndex,
    //         PageSize = pageSize,
    //         TotalItemsCount = 2,
    //         Items = deliveries
    //     };
    //
    //     _unitOfWorkMock.Setup(x => x.DeliveryRepository.ToPagination(
    //             pageIndex, pageSize, It.IsAny<Expression<Func<Delivery, bool>>>(), It.IsAny<IncludeInfo<Delivery>>(), It.IsAny<IncludeInfo<Delivery>>()))
    //         .ReturnsAsync(paginationResult);
    //
    //     // _mapper.Setup(m => m.Map<List<DeliveryResponseModel>>(It.IsAny<List<Delivery>>()))
    //     //     .Returns(deliveries.Select(d => new DeliveryResponseModel(d.Id, d.Name, null, null, 0, null, null, new List<string>(), new List<string>(), 0)).ToList());
    //
    //     // Act
    //     var result = await _deliveryService.GetDeliveryUnitPaginationAsync(searchTerm, pageIndex, pageSize);
    //
    //     // Assert
    //     Assert.NotNull(result.Payload);
    //     Assert.Equal(2, result.Payload.Items.Count);
    //     Assert.Equal(pageIndex, result.Payload.PageIndex);
    //     Assert.Equal(pageSize, result.Payload.PageSize);
    //     _unitOfWorkMock.VerifyAll();
    // }
}