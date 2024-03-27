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
    /// API FOR Super Admin 
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpGet,Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetSupplyAssigment()
    {
        var response = await _supplyAssigmentService.GetSupplyAssigment();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    /// <summary>
    /// API FOR Super Admin 
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost,Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreateSupplyAssigment(CreateSupplyAssigment requestModel)
    {
        var response = await _supplyAssigmentService.CreateSupplyAssigment(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin (xóa quan hệ gán giữa thằng ncc và đối tác)
    /// </summary>
    /// <param name="supplierId"></param>
    /// <param name="partnerId"></param>
    /// <returns></returns>
    [HttpDelete("supplier/{supplierId}/partner/{partnerId}")]
    public async Task<IActionResult> Remove(Guid supplierId,Guid partnerId)
    {
        var response = await _supplyAssigmentService.RemoveSupplyAssigment(supplierId, partnerId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}