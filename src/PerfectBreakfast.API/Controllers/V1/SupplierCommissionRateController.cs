using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/suppliercommissionrates")]
public class SupplierCommissionRateController : BaseController
{
    private readonly ISupplierCommissionRateService _supplierCommissionRate;

    public SupplierCommissionRateController(ISupplierCommissionRateService supplierCommissionRate)
    {
        _supplierCommissionRate = supplierCommissionRate;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSupplierCommissionRates()
    {
        var response = await _supplierCommissionRate.GetSupplierCommissionRates();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierCommissionRate(Guid id)
    {
        var response = await _supplierCommissionRate.GetSupplierCommissionRateId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSupplierCommissionRate(CreateSupplierCommissionRateRequest supplierCommissionRateRequest)
    {
        var response = await _supplierCommissionRate.CreateSupplierCommissionRate(supplierCommissionRateRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplierCommissionRate(Guid id, UpdateSupplierCommissionRateRequest supplierCommissionRateRequest)
    {
        var response = await _supplierCommissionRate.UpdateSupplierCommissionRate(id, supplierCommissionRateRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("{id}/suppliercomissionrate-status")]
    public async Task<IActionResult> DeleteSupplierCommissionRate(Guid id)
    {
        var response = await _supplierCommissionRate.DeleteCSupplierCommissionRate(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetSupplierCommissionRatePagination(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _supplierCommissionRate.GetSupplierCommissionRatePaginationAsync(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _supplierCommissionRate.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}