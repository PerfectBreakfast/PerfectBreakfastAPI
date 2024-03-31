using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/package")]
public class PackageController : BaseController
{
    private readonly IPackageService _packageService;

    public PackageController(IPackageService packageService)
    {
        _packageService = packageService;
    }
    
    /// <summary>
    /// API For Partner Admin-API lấy danh sách package cho partner
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("partner"), Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetPackageForPartner()
    {
        var response = await _packageService.GetPackageForPartner();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}