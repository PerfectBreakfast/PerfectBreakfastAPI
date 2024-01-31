using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Services;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/deliveries")]
public class DeliveryController : BaseController
{
    private readonly IDeliveryService _deliveryService;

    public DeliveryController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDeliveryUnits()
    {
        var response = await _deliveryService.GetDeliveries();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateDeliveryUnit(CreateDeliveryUnitRequest requestModel)
    {
        var response = await _deliveryService.CreateDelivery(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeliveryUnit(Guid id, UpdateDeliveryUnitRequest requestModel)
    {
        var response = await _deliveryService.UpdateDelivery(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveDeliveryUnit(Guid id)
    {
        var response = await _deliveryService.RemoveDelivery(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeliveryUnitId(Guid id)
    {
        var response = await _deliveryService.GetDeliveryId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetDeliveryUnitPagination(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var response = await _deliveryService.GetDeliveryUnitPaginationAsync(searchTerm,pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}