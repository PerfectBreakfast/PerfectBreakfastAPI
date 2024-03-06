using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/deliveries")]
public class DeliveryController : BaseController
{
    private readonly IDeliveryService _deliveryService;

    public DeliveryController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    /// <summary>
    /// Api for All
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetDeliveries()
    {
        var response = await _deliveryService.GetDeliveries();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreateDelivery(CreateDeliveryRequest requestModel)
    {
        var response = await _deliveryService.CreateDelivery(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> UpdateDelivery(Guid id, UpdateDeliveryRequest requestModel)
    {
        var response = await _deliveryService.UpdateDelivery(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> RemoveDelivery(Guid id)
    {
        var response = await _deliveryService.RemoveDelivery(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for All
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeliveryId(Guid id)
    {
        var response = await _deliveryService.GetDeliveryId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetDeliveryPagination(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var response = await _deliveryService.GetDeliveryUnitPaginationAsync(searchTerm,pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}