using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/suppliercommissionrates")]
public class SupplierCommissionRateController : BaseController
{
    private readonly ISupplierCommissionRateService _supplierCommissionRate;

    public SupplierCommissionRateController(ISupplierCommissionRateService supplierCommissionRate)
    {
        _supplierCommissionRate = supplierCommissionRate;
    }
    
  

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierCommissionRate(Guid id)
    {
        var response = await _supplierCommissionRate.GetSupplierCommissionRateId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    /// <summary>
    /// get more food by supplier
    /// </summary>
    /// <param name="supplierId"></param>
    /// <returns></returns>
    [HttpGet("food/{supplierId}")]
    public async Task<IActionResult> GetSupplierMoreFood(Guid supplierId)
    {
        var response = await _supplierCommissionRate.GetSupplierMoreFood(supplierId);
        return response.IsError ? BadRequest(response.Errors) : Ok(response.Payload);
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

    

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _supplierCommissionRate.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
}