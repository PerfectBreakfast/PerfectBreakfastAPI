using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/dashboards")]
public class DashboardController : BaseController
{

    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    
    /// <summary>
    /// API for Super Admin - Lấy Top 10 Combo/món ăn theo ngày
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("top-combo-food/from-date-to-date"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> TopComboAndFood(DateOnly? fromDate, DateOnly? toDate)
    {
        var response = await _dashboardService.GetTopComboAndFood(fromDate, toDate);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API for Super Admin - Trả về 10 công ty có số lượng đơn đặt cao nhất
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("top-company/from-date-to-date"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetTopCompany(DateOnly? fromDate, DateOnly? toDate)
    {
        var response = await _dashboardService.GetTop10CompanyOrder(fromDate, toDate);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API for Super Admin (thống kê trạng thái đơn hàng theo ngày) 
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("order-statistic/from-date-to-date"), Authorize(policy: ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetOrderStatistic(DateOnly? fromDate, DateOnly? toDate)
    {
        var response = await _dashboardService.OrderStatistic(fromDate, toDate);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API for Super Admin (thống kê số lượng đơn hàng theo ngày)
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("order-amount/from-date-to-date"), Authorize(policy: ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetOrderByDateStatistic(DateOnly? fromDate, DateOnly? toDate)
    {
        var response = await _dashboardService.OrderAmountByDateStatistic(fromDate, toDate);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}