using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/deliveryunits")]
public class DeliveryUnitController : BaseController
{
    private readonly IDeliveryUnitService _deliveryUnitService;

    public DeliveryUnitController(IDeliveryUnitService deliveryUnitService)
    {
        _deliveryUnitService = deliveryUnitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDeliveryUnits()
    {
        var response = await _deliveryUnitService.GetDeliveries();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}