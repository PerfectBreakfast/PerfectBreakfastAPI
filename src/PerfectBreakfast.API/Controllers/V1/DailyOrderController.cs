using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DailyOrder.Request;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/daily-orders")]
public class DailyOrderController : BaseController
{
    private readonly IDailyOrderService _dailyOrderService;
    private readonly IExportExcelService _exportExcelService;
    private readonly ISupplierFoodAssignmentService _foodAssignmentService;

    public DailyOrderController(IDailyOrderService dailyOrderService, IExportExcelService exportExcelService, ISupplierFoodAssignmentService foodAssignmentService)
    {
        _dailyOrderService = dailyOrderService;
        _exportExcelService = exportExcelService;
        _foodAssignmentService = foodAssignmentService;
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
    /// Api for Super Admin
    /// </summary>
    /// <param name="status"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <remarks>
    ///  0- Initial 1-Processing 2-Divided 3-Cooking 4-Waiting 5-Delivering 6-Complete  7&8 không cần
    /// </remarks> 
    [HttpGet("pagination"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetDailyOrderPagination(int pageIndex = 0, int pageSize = 10,DailyOrderStatus? status = null)
    {
        var response = await _dailyOrderService.GetDailyOrderPaginationAsync(status,pageIndex, pageSize);
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
    /// API For Partner Admin - Delivery Admin (Tải thống kê số lượng đơn hàng và các combo/food thuộc các doanh nghiệp theo ngày)
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("statistic")]
    [Authorize(Roles = $"{ConstantRole.PARTNER_ADMIN}, {ConstantRole.DELIVERY_ADMIN}")]
    public async Task<IActionResult> GetDailyOrderStatistic(DateOnly? fromDate, DateOnly? toDate)
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
    
    /// <summary>
    /// API For Super Admin (Tải thống kê số lượng đơn hàng và các combo/food thuộc các doanh nghiệp theo ngày)
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("statistic/super-admin")]
    
    public async Task<IActionResult> GetDailyOrderStatisticForSuperAdmin(DateOnly? fromDate, DateOnly? toDate)
    {
        var response = await _dailyOrderService.GetDailyOrderForDownload(fromDate, toDate);
        var assignmentResponse = await _foodAssignmentService.GetSupplierFoodAssignmentsForSuperAdmin(fromDate, toDate);
        if (response.IsError || assignmentResponse.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        var content = _exportExcelService.DownloadDailyOrderStatisticForSuperAdmin(response.Payload, assignmentResponse.Payload);
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