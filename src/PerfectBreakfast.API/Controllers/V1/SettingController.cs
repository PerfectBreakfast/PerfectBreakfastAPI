using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SettingModels.Request;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/hangfire/settings")]
public class SettingController : BaseController
{
    private readonly IHangfireSettingService _hangfireSettingService;

    public SettingController(IHangfireSettingService hangfireSettingService)
    {
        _hangfireSettingService = hangfireSettingService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiVersionNeutral]
    public async Task<IActionResult> SetupTimeAutoCreateAndUpdateDailyOrder(TimeSettingRequest request)
    {
        var response = await _hangfireSettingService.UpdateTime(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GAGA()
    {
        return Ok("hahahaha");
    }
}