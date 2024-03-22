using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/suppliers")]
public class SupplierController : BaseController
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    /// <summary>
    /// API For Super Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize]
    public async Task<IActionResult> GetSuppliers()
    {
        var response = await _supplierService.GetSuppliers();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Super Admin
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetPagination( string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var response = await _supplierService.GetPaginationAsync(searchTerm,pageIndex,pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost,Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreateSupplier(CreateSupplierRequestModel requestModel)
    {
        var response = await _supplierService.CreateSupplier(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> UpdateSupplier(Guid id, UpdateSupplierRequestModel requestModel)
    {
        var response = await _supplierService.UpdateSupplier(id,requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> RemoveSupplier(Guid id)
    {
        var response = await _supplierService.RemoveSupplier(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Supplier Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]

    public async Task<IActionResult> GetSupplierId(Guid id)
    {
        var response = await _supplierService.GetSupplierId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API for Partner Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet("partner"),Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetSupplierByPartner(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var response = await _supplierService.GetSupplierByPartner(searchTerm, pageIndex, pageSize);
        return response.IsError? HandleErrorResponse(response.Errors) : Ok(response?.Payload);
    }
    
    /// <summary>
    /// API for Partner Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet("all/partner"),Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetSupplierByPartner()
    {
        var response = await _supplierService.GetAllSupplierByPartner();
        return response.IsError? HandleErrorResponse(response.Errors) : Ok(response?.Payload);
    }
    
    /// <summary>
    /// API for Partner Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet("food/{id:guid}"),Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetSupplierByFood(Guid id)
    {
        var response = await _supplierService.GetSupplierByFood(id);
        return response.IsError? HandleErrorResponse(response.Errors) : Ok(response?.Payload);
    }
}