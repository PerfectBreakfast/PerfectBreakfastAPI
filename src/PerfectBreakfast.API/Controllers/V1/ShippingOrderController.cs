using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/shippingorders")]
public class ShippingOrderController : BaseController
{
    private readonly IShippingOrderService _shippingOrderService;

    public ShippingOrderController(IShippingOrderService shippingOrderService)
    {
        _shippingOrderService = shippingOrderService;
    }
    /// <summary>
    /// Api for Delivery Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet("deliveryadmin"), Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> GetShippingOrder()
    {
        var response = await _shippingOrderService.GetAllShippingOrdersWithDetails();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    /// <summary>
    /// Api for Delivery Admin 
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost("deliveryadmin"), Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> CreateShippingOrder(CreateShippingOrderRequest requestModel)
    {
        var response = await _shippingOrderService.CreateShippingOrder(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Delivery Staff 
    /// </summary>
    /// <returns></returns>
    [HttpGet("deliverystaff"),Authorize(Policy = ConstantRole.RequireDeliveryStaffRole)]
    public async Task<IActionResult> GetDailyOrderByDeliveryStaff()
    {
        var response = await _shippingOrderService.GetShippingOrderByDeliveryStaff();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Delivery Admin 
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("deliveryadmin"), Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> UpdateStatusShippingOrder(Guid shippingOrderId, UpdateStatusShippingOrderRequest updateStatusShippingOrderRequest)
    {
        var response = await _shippingOrderService.UpdateShippingOrderStatus(shippingOrderId, updateStatusShippingOrderRequest); // Pass both ID and DTO to the service
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}