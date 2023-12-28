using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierModels.Request;

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
    /// API For Supper Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetSuppliers()
    {
        var response = await _supplierService.GetSuppliers();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Supper Admin
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination")]
    public async Task<IActionResult> GetPagination(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _supplierService.GetPaginationAsync(pageIndex,pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Supper Admin
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateSupplier(CreateSupplierRequestModel requestModel)
    {
        var response = await _supplierService.CreateSupplier(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Supper Admin
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(Guid id, UpdateSupplierRequestModel requestModel)
    {
        var response = await _supplierService.UpdateSupplier(id,requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Supper Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveSupplier(Guid id)
    {
        var response = await _supplierService.RemoveSupplier(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}