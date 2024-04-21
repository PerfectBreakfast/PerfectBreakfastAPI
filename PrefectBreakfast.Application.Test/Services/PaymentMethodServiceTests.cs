using FluentAssertions;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Request;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class PaymentMethodServiceTests : SetupTest
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodServiceTests()
    {
        _paymentMethodService = new PaymentMethodService(_unitOfWorkMock.Object, _mapper);
    }
    
    [Fact]
    public async Task CreatePaymentMethod_AddsPaymentMethod_Successfully()
    {
        // Arrange
        var request = new PaymentMethodRequest { Name = "Credit Card" };
        var paymentMethod = new PaymentMethod { Name = request.Name };

        //_mapper.Setup(m => m.Map<PaymentMethod>(request)).Returns(paymentMethod);
        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.AddAsync(It.IsAny<PaymentMethod>())).ReturnsAsync(paymentMethod);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _paymentMethodService.CreatePaymentMethod(request);

        // Assert
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.AddAsync(It.IsAny<PaymentMethod>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        //Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task CreatePaymentMethod_ReturnsError_OnSaveException()
    {
        // Arrange
        var request = new PaymentMethodRequest { Name = "Credit Card" };
        var paymentMethod = new PaymentMethod { Name = request.Name };

        //_mapper.Setup(m => m.Map<PaymentMethod>(request)).Returns(paymentMethod);
        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.AddAsync(It.IsAny<PaymentMethod>())).ReturnsAsync(paymentMethod);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ThrowsAsync(new Exception("Saving changes failed"));

        // Act
        var result = await _paymentMethodService.CreatePaymentMethod(request);

        // Assert
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.AddAsync(It.IsAny<PaymentMethod>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        //Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task DeletePaymentMethod_DeletesSuccessfully_WhenPaymentMethodExists()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var paymentMethod = new PaymentMethod { Id = paymentMethodId, Name = "Credit Card" };

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetByIdAsync(paymentMethodId)).ReturnsAsync(paymentMethod);
        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.SoftRemove(paymentMethod));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _paymentMethodService.DeletePaymentMethod(paymentMethodId);

        // Assert
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.GetByIdAsync(paymentMethodId), Times.Once);
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.SoftRemove(It.IsAny<PaymentMethod>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeletePaymentMethod_ReturnsError_OnSaveException()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var paymentMethod = new PaymentMethod { Id = paymentMethodId, Name = "Credit Card" };

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetByIdAsync(paymentMethodId))
            .ThrowsAsync(new Exception());
        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.SoftRemove(paymentMethod));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ThrowsAsync(new Exception("Saving changes failed"));

        // Act
        var result = await _paymentMethodService.DeletePaymentMethod(paymentMethodId);

        // Assert
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.GetByIdAsync(paymentMethodId), Times.Once);
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.SoftRemove(It.IsAny<PaymentMethod>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
    }
    
    [Fact]
    public async Task GetPaymentMethod_ReturnsPaymentMethod_WhenItExists()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var paymentMethod = new PaymentMethod { Id = paymentMethodId, Name = "Credit Card" };
        var paymentMethodResponse = new PaymentMethodResponse { Id = paymentMethodId, Name = "Credit Card" };

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetByIdAsync(paymentMethodId)).ReturnsAsync(paymentMethod);
        //_mapper.Setup(m => m.Map<PaymentMethodResponse>(paymentMethod)).Returns(paymentMethodResponse);
        _mapper.Map<PaymentMethodResponse>(paymentMethod);

        // Act
        var result = await _paymentMethodService.GetPaymentMethod(paymentMethodId);

        // Assert
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.GetByIdAsync(paymentMethodId), Times.Once);
        //Assert.True(result.IsSuccess);
        Assert.Equal(paymentMethodResponse, result.Payload);
    }
    
    [Fact]
    public async Task GetPaymentMethod_ReturnsError_OnSaveException()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var paymentMethod = new PaymentMethod { Id = paymentMethodId, Name = "Credit Card" };
        var paymentMethodResponse = new PaymentMethodResponse { Id = paymentMethodId, Name = "Credit Card" };

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetByIdAsync(paymentMethodId))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _paymentMethodService.GetPaymentMethod(paymentMethodId);

        // Assert
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.GetByIdAsync(paymentMethodId), Times.Once);
    }
    
    [Fact]
    public async Task UpdatePaymentMethod_UpdatesData_WhenPaymentMethodExists()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var request = new PaymentMethodRequest { Name = "Updated Card"};
        var paymentMethod = new PaymentMethod { Id = paymentMethodId, Name = "Credit Card" };

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetByIdAsync(paymentMethodId)).ReturnsAsync(paymentMethod);
        //_mapper.Setup(m => m.Map(request, paymentMethod)).Returns(paymentMethod);
        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.Update(paymentMethod));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _paymentMethodService.UpdatePaymentMethod(paymentMethodId, request);

        // Assert
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.Update(It.IsAny<PaymentMethod>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        //Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task UpdatePaymentMethod_ReturnsError_OnSaveException()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var request = new PaymentMethodRequest { Name = "Updated Card"};
        var paymentMethod = new PaymentMethod { Id = paymentMethodId, Name = "Credit Card" };

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetByIdAsync(paymentMethodId)).ReturnsAsync(paymentMethod);
        //_mapper.Setup(m => m.Map(request, paymentMethod)).Returns(paymentMethod);
        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.Update(paymentMethod));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ThrowsAsync(new Exception());

        // Act
        var result = await _paymentMethodService.UpdatePaymentMethod(paymentMethodId, request);

        // Assert
        _unitOfWorkMock.Verify(x => x.PaymentMethodRepository.Update(It.IsAny<PaymentMethod>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        //Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task GetPaymentMethods_ReturnsAllPaymentMethods()
    {
        // Arrange
        var paymentMethods = new List<PaymentMethod>
        {
            new PaymentMethod { Id = Guid.NewGuid(), Name = "Credit Card"},
            new PaymentMethod { Id = Guid.NewGuid(), Name = "Debit Card" }
        };

        var paymentMethodResponses = paymentMethods.Select(pm => new PaymentMethodResponse
        {
            Id = pm.Id,
            Name = pm.Name,
        }).ToList();

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetAllAsync()).ReturnsAsync(paymentMethods);
        //_mapper.Setup(m => m.Map<List<PaymentMethodResponse>>(paymentMethods)).Returns(paymentMethodResponses);

        // Act
        var result = await _paymentMethodService.GetPaymentMethods();

        // Assert
        _unitOfWorkMock.Verify(u => u.PaymentMethodRepository.GetAllAsync(), Times.Once);
        result.Should().NotBeNull();
        result.Payload.Should().HaveCount(paymentMethods.Count);
        result.Payload.Should().BeEquivalentTo(paymentMethodResponses, options => options.ComparingByMembers<PaymentMethodResponse>());
    }
    
    [Fact]
    public async Task GetPaymentMethods_ReturnsError_WhenExceptionThrown()
    {
        // Arrange
        var expectedErrorMessage = "An error occurred retrieving payment methods.";
        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetAllAsync()).ThrowsAsync(new Exception(expectedErrorMessage));

        // Act
        var result = await _paymentMethodService.GetPaymentMethods();

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Be(expectedErrorMessage);
    }
    
    [Fact]
    public async Task Delete_ReturnsSuccess_WhenPaymentMethodExists()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var paymentMethod = new PaymentMethod { Id = paymentMethodId, Name = "Visa" };

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetByIdAsync(paymentMethodId))
            .ReturnsAsync(paymentMethod);

        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.Remove(paymentMethod));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);  // Assuming 1 row affected

        // Act
        var result = await _paymentMethodService.Delete(paymentMethodId);

        // Assert
        _unitOfWorkMock.Verify(u => u.PaymentMethodRepository.Remove(It.IsAny<PaymentMethod>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Delete_ReturnsError_WhenExceptionThrown()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var expectedErrorMessage = "Database connection error.";
        _unitOfWorkMock.Setup(u => u.PaymentMethodRepository.GetByIdAsync(paymentMethodId))
            .ThrowsAsync(new Exception(expectedErrorMessage));

        // Act
        var result = await _paymentMethodService.Delete(paymentMethodId);

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Be(expectedErrorMessage);
    }
}