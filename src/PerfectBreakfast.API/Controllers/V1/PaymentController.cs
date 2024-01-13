using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using PerfectBreakfast.Application.Interfaces;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IPayOsService _payOsService;

    public PaymentController(ILogger<PaymentController> logger,IPayOsService payOsService)
    {
        _logger = logger;
        _payOsService = payOsService;
    }
    
    [HttpPost("payos_transfer_handler")]
    public async Task<IActionResult> payOSTransferHandler(WebhookType body)
    {
        _logger.LogInformation("Receive data from payOS");
        var response = await _payOsService.HandleWebhook(body);
        return Ok(new {Success = response});
    }
}
    

