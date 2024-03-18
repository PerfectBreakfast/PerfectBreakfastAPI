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
    [HttpGet("delivery-staff"),Authorize(Policy = ConstantRole.RequireDeliveryStaffRole)]
    public async Task<IActionResult> GetDailyOrderByDeliveryStaff()
    {
        var response = await _shippingOrderService.GetShippingOrderByDeliveryStaff();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Delivery Staff- (API xác nhận món lấy hàng đi giao)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("confirmation/{id:guid}")]
    [Authorize(Roles = "DELIVERY ADMIN, DELIVERY STAFF")]
    public async Task<IActionResult> ConfirmStatusShippingOrder(Guid id)
    {
        var response = await _shippingOrderService.ConfirmShippingOrderByShipper(id); // Pass both ID and DTO to the service
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Delivery Staff-API lấy daily order cần giao 
    /// </summary>
    /// <returns></returns>
    [HttpGet("daily-order/pending-status"),Authorize(Policy = ConstantRole.RequireDeliveryStaffRole)]
    public async Task<IActionResult> GetDailyOrderPendingByDeliveryStaff()
    {
        var response = await _shippingOrderService.GetDailyOrderByShipper();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Delivery Staff-API lấy lịch sử daily order
    /// </summary>
    /// <returns></returns>
    [HttpGet("delivery-staff/history-order"),Authorize(Policy = ConstantRole.RequireDeliveryStaffRole)]
    public async Task<IActionResult> GetAllDailyOrderByDeliveryStaff()
    {
        var response = await _shippingOrderService.GetHistoryDailyOrderByShipper();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Delivery Admin-API để lấy staff theo daily order
    /// </summary>
    /// <returns></returns>
    [HttpGet("delivery-staff/daily-order/{id:guid}"), Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> GetDeliveryStaffByDailyOrder(Guid id)
    {
        var response = await _shippingOrderService.GetSDeliveryStaffByDailyOrder(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}