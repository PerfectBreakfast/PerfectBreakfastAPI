using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DailyOrder.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/daily-orders")]
public class DailyOrderController : BaseController
{
    private readonly IDailyOrderService _dailyOrderService;
    private readonly IExportExcelService _exportExcelService;

    public DailyOrderController(IDailyOrderService dailyOrderService, IExportExcelService exportExcelService)
    {
        _dailyOrderService = dailyOrderService;
        _exportExcelService = exportExcelService;
    }

    /// <summary>
    /// Api For All
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetDailyOrders()
    {
        var response = await _dailyOrderService.GetDailyOrders();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Super Admin
    /// </summary>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreateDailyOrder(DailyOrderRequest dailyOrderRequest)
    {
        var response = await _dailyOrderService.CreateDailyOrder(dailyOrderRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin 
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetDailyOrderPagination(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _dailyOrderService.GetDailyOrderPaginationAsync(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Super Admin
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPut("/Completion/{id:guid}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CompleteDailyOrder(Guid id)
    {
        var response = await _dailyOrderService.CompleteDailyOrder(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Super Admin 
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPut("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> UpdateDailyOrder(Guid id, UpdateDailyOrderRequest updateDailyOrderRequest)
    {
        var response = await _dailyOrderService.UpdateDailyOrder(id, updateDailyOrderRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Partner Admin-API lấy danh sách daily order trong trạng thái chờ phân phối
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("partner/process"), Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetDailyOrderProcessingByPartner(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _dailyOrderService.GetDailyOrderProcessingByPartner(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Delivery Admin-API lấy danh sách daily order trong trạng thái chờ phân phối
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("delivery/process"), Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> GetDailyOrderProcessingByDelivery(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _dailyOrderService.GetDailyOrderProcessingByDelivery(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Partner Admin (Lấy ra danh sách các dailyOrder của partner đó)
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("partner"), Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetDailyOrderByPartner(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _dailyOrderService.GetDailyOrderByPartner(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Delivery Admin (Lấy ra danh sách các dailyOrder của DeliveryAdmin đó)
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("delivery"), Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> GetDailyOrderByDelivery(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _dailyOrderService.GetDailyOrderByDelivery(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Super Admin
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("Statistic")]
    public async Task<IActionResult> GetDailyOrderStatistic(DateOnly fromDate, DateOnly toDate)
    {
        var response = await _dailyOrderService.GetDailyOrderForDownload(fromDate, toDate);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        var content = _exportExcelService.DownloadDailyOrderStatistic(response.Payload);
        if (content == null)
        {
            return NotFound("Some thing wrong");
        }
        else
        {
            var fileName = $"Thống kê đơn hàng từ {fromDate} đến {toDate}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}