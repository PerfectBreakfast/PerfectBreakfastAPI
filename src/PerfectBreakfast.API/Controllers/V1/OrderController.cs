﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/orders")]
public class OrderController : BaseController
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// API for Customer
    /// </summary>
    /// <param name="orderRequest"></param>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = ConstantRole.RequireCustomerRole)]
    public async Task<IActionResult> CreateOrder(OrderRequest orderRequest)
    {
        var response = await _orderService.CreateOrder(orderRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Customer (show lại link thanh toán cho customer)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}/paymentlink"),Authorize(Policy = ConstantRole.RequireCustomerRole)]
    public async Task<IActionResult> GetLinkContinuePayment(Guid id)
    {
        var response = await _orderService.GetLinkContinuePayment(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Customer (Hủy thanh toán đơn hàng)
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    [HttpPut("{orderCode}/cancel"),Authorize(Policy = ConstantRole.RequireCustomerRole)]
    public async Task<IActionResult> CancelOrder(long orderCode)
    {
        var response = await _orderService.CancelOrder(orderCode);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var response = await _orderService.GetOrders();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api For All login 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}"), Authorize]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var response = await _orderService.GetOrder(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Customer (xem danh sách lịch sử đơn đặt hàng)
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <returns></returns>
    [HttpGet("history"), Authorize(policy: ConstantRole.RequireCustomerRole)]
    public async Task<IActionResult> GetOrderHistoryByCustomer(int pageNumber = 1)
    {
        var response = await _orderService.GetOrderHistory(pageNumber);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api for Delivery Staff (xem lịch sử các đơn hàng đã quét)
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <returns></returns>
    [HttpGet("deliverystaff/history"), Authorize(policy: ConstantRole.RequireDeliveryStaffRole)]
    public async Task<IActionResult> GetOrderHistoryByDeliveryStaff(int pageNumber = 1)
    {
        var response = await _orderService.GetOrderHistoryByDeliveryStaff(pageNumber);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin, Customer
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateOrderRequest"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{ConstantRole.CUSTOMER},{ConstantRole.SUPER_ADMIN}")]
    public async Task<IActionResult> UpdateOrder(Guid id, UpdateOrderRequest updateOrderRequest)
    {
        var response = await _orderService.UpdateOrder(id, updateOrderRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination"), Authorize(policy: ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetOrderPagination(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _orderService.GetOrderPaginationAsync(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Customer, Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{ConstantRole.CUSTOMER},{ConstantRole.SUPER_ADMIN}")]
    public async Task<IActionResult> RemoveOrder(Guid id)
    {
        var response = await _orderService.DeleteOrder(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Delivery Staff or Delivery Admin (sau khi quét mã và xác nhận đã giao đơn cho khách)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch("{id}/status-complete")]
    [Authorize(Roles = $"{ConstantRole.DELIVERY_ADMIN},{ConstantRole.DELIVERY_STAFF}")]
    public async Task<IActionResult> CompleteOrder(Guid id)
    {
        var response = await _orderService.CompleteOrder(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin (thống kê)
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("statistic"), Authorize(policy: ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetOrderPagination(DateOnly fromDate, DateOnly toDate)
    {
        var response = await _orderService.OrderStatistic(fromDate, toDate);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// Api For All login 
    /// </summary>
    /// <param name="dailyOrderId"></param>
    /// <returns></returns>
    [HttpGet("daily-order/{dailyOrderId:guid}"), Authorize]
    public async Task<IActionResult> GetOrderByDailyOrder(Guid dailyOrderId)
    {
        var response = await _orderService.GetOrderByDailyOrder(dailyOrderId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}