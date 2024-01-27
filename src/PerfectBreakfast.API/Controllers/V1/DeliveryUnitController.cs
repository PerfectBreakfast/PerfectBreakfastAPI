using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Services;

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
    
    [HttpPost]
    public async Task<IActionResult> CreateDeliveryUnit(CreateDeliveryUnitRequest requestModel)
    {
        var response = await _deliveryUnitService.CreateDelivery(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeliveryUnit(Guid id, UpdateDeliveryUnitRequest requestModel)
    {
        var response = await _deliveryUnitService.UpdateDelivery(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveDeliveryUnit(Guid id)
    {
        var response = await _deliveryUnitService.RemoveDelivery(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeliveryUnitId(Guid id)
    {
        var response = await _deliveryUnitService.GetDeliveryId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetDeliveryUnitPagination(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var response = await _deliveryUnitService.GetDeliveryUnitPaginationAsync(searchTerm,pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}