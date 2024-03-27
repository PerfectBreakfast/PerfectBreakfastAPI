using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PartnerModels.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/partners")]
public class PartnerController : BaseController
{
    private readonly IPartnerService _partnerService;
    
    public PartnerController(IPartnerService partnerService)
    {
        _partnerService = partnerService;
    }
    
    /// <summary>
    /// API FOR SUPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpGet,Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetPartners()
    {
        var response = await _partnerService.GetPartners();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API FOR SUPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpPost,Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreatePartner(CreatePartnerRequest requestModel)
    {
        var response = await _partnerService.CreatePartner(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API FOR SUPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> UpdatePartner(Guid id, UpdatePartnerRequest requestModel)
    {
        var response = await _partnerService.UpdatePartner(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API FOR SUPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> RemovePartner(Guid id)
    {
        var response = await _partnerService.RemovePartner(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API FOR SUPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetPartnerId(Guid id)
    {
        var response = await _partnerService.GetPartnerId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API FOR SUPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpGet("pagination"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetPartnerPagination(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var response = await _partnerService.GetPartnerPaginationAsync(searchTerm,pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API FOR SUPER ADMIN
    /// </summary>
    /// <returns></returns>
    [HttpGet ("supplier-assignment/{supplierId:guid}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetPartnerToSupplierAssign(Guid supplierId)
    {
        var response = await _partnerService.AssignPartnerToSupplier(supplierId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}