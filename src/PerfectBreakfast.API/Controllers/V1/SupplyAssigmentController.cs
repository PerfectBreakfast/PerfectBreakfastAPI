using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;
[Route("api/v{version:apiVersion}/supplyassigments")]
public class SupplyAssigmentController : BaseController
{
    private readonly ISupplyAssigmentService _supplyAssigmentService;

    public SupplyAssigmentController(ISupplyAssigmentService supplyAssigmentService)
    {
        _supplyAssigmentService = supplyAssigmentService;
    }
    /// <summary>
    /// API FOR PARTNER ADMIN
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetSupplyAssigment()
    {
        var response = await _supplyAssigmentService.GetSupplyAssigment();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    /// <summary>
    /// API FOR PARTNER ADMIN
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost,Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> CreateSupplyAssigment(CreateSupplyAssigment requestModel)
    {
        var response = await _supplyAssigmentService.CreateSupplyAssigment(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}