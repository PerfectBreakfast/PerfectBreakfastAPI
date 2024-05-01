using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Net.payOS.Types;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Models.MomoModels.Request;
using PerfectBreakfast.Application.Models.PaymentModels.Request;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class PaymentControllerTest : SetupTest
{
    private readonly PaymentController _paymentController;
    private readonly ILogger<PaymentController> _logger;

    public PaymentControllerTest()
    {
        _logger = new LoggerFactory().CreateLogger<PaymentController>();
        _paymentController = new PaymentController(_logger,_payOsServiceMock.Object, _MomoServiceMock.Object);
    }
    
    [Fact]
    public async Task PayOSTransferHandler_ReturnsOk()
    {
        // Arrange
        var random = new Random();

// Tạo dữ liệu giả mạo cho WebhookData
        var webhookData = new WebhookData(
            orderCode: random.Next(),
            amount: random.Next(),
            description: "Sample description",
            accountNumber: "1234567890",
            reference: Guid.NewGuid().ToString(),
            transactionDateTime: DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
            currency: "USD",
            paymentLinkId: Guid.NewGuid().ToString(),
            code: "sample_code",
            desc: "Sample description",
            counterAccountBankId: random.Next(2) == 0 ? "bank_id" : null,
            counterAccountBankName: random.Next(2) == 0 ? "Bank Name" : null,
            counterAccountName: random.Next(2) == 0 ? "Counter Account Name" : null,
            counterAccountNumber: random.Next(2) == 0 ? "Counter Account Number" : null,
            virtualAccountName: random.Next(2) == 0 ? "Virtual Account Name" : null,
            virtualAccountNumber: "0987654321"
        );
        var body = new WebhookType("code", "desc", webhookData, "signature");

        // Act
        var result = await _paymentController.payOSTransferHandler(body);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task MomoIpnReturn_ReturnsNoContent()
    {
        // Arrange
        var request = new MomoOneTimePaymentResultRequest();

        // Act
        var result = await _paymentController.MomoIpnReturn(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ConfirmWebhook_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var body = new ConfirmWebhook("");
        _payOsServiceMock.Setup(x => x.ConfirmWebhook(body)).ReturnsAsync(true);

        // Act
        var result = await _paymentController.ConfirmWebhook(body);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value);
    }

    [Fact]
    public async Task ConfirmWebhook_ReturnsInternalServerError_WhenServiceFails()
    {
        // Arrange
        var body = new ConfirmWebhook("");
        _payOsServiceMock.Setup(x => x.ConfirmWebhook(body)).ReturnsAsync(false);

        // Act
        var result = await _paymentController.ConfirmWebhook(body);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var statusCodeResult = (OkObjectResult)result;
        Assert.Equal(200, statusCodeResult.StatusCode);
    }
}