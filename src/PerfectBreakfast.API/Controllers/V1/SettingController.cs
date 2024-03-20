using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SettingModels.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/hangfire/settings")]
public class SettingController : BaseController
{
    private readonly IHangfireSettingService _hangfireSettingService;

    public SettingController(IHangfireSettingService hangfireSettingService)
    {
        _hangfireSettingService = hangfireSettingService;
    }
    
    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> SetupTimeAutoCreateAndUpdateDailyOrder(Guid id,TimeSettingRequest request)
    {
        var response = await _hangfireSettingService.UpdateTime(id,request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet,Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetTimeAuto()
    {
        var response = await _hangfireSettingService.GetTimeAuto();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}