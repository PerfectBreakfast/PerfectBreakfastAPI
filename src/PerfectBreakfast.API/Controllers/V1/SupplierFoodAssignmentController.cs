using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/supplierfoodassigments")]
public class SupplierFoodAssignmentController : BaseController
{
    private readonly ISupplierFoodAssignmentService _supplierFoodAssignmentService;

    public SupplierFoodAssignmentController(ISupplierFoodAssignmentService supplierFoodAssignmentService)
    {
        _supplierFoodAssignmentService = supplierFoodAssignmentService;
    }

    /// <summary>
    /// API For Partner Admin
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost, Authorize(policy:ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> CreateSupplierFoodAssignment(SupplierFoodAssignmentsRequest supplierFoodAssignmentRequest)
    {
        var response = await _supplierFoodAssignmentService.CreateSupplierFoodAssignment(supplierFoodAssignmentRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Partner Admin
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("partner"), Authorize(policy:ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetSupplierFoodAssignmentByPartner(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentByPartner(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Supplier Admin
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("supplier")]
    [Authorize(Roles = "SUPPLIER ADMIN")]
    public async Task<IActionResult> GetSupplierFoodAssignmentBySupplier(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentBySupplier(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Partner Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}/status-completion"), Authorize(policy:ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> CompleteFoodAssignmentByPartner(Guid id)
    {
        var response = await _supplierFoodAssignmentService.CompleteFoodAssignment(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API For Supplier Admin
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"> 0: Declined, 1: Confirmed</param>
    /// <returns></returns>
    [HttpPut("{id}/status-confirmation")]
    [Authorize(Roles = "SUPPLIER ADMIN")]
    public async Task<IActionResult> ConfirmFoodAssignmentBySupplier(Guid id, int status)
    {
        var response = await _supplierFoodAssignmentService.ChangeStatusFoodAssignment(id, status);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    } 
    
    /// <summary>
    /// API For Partner Admin
    /// </summary>
    /// <returns></returns>
    [HttpPut, Authorize(policy:ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> UpdateSupplierCommissionRate(UpdateSupplierFoodAssignment updateSupplierFoodAssignment)
    {
        var response = await _supplierFoodAssignmentService.UpdateSupplierFoodAssignment(updateSupplierFoodAssignment);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Supplier Admin
    /// </summary>
    /// <param name="bookingDate"></param>
    /// <returns></returns>
    [HttpGet("download-excel")]
    //[Authorize(Roles = "SUPPLIER ADMIN")]
    public async Task<IActionResult> DownloadSupplierFoodAssignment(DateOnly bookingDate)
    {
        
        var response = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentsForDownload(bookingDate);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        var content = _supplierFoodAssignmentService.DownloadSupplierFoodAssignmentExcel(response.Payload[0]);
        if (content == null)
        {
            return NotFound("Some thing wrong");
        }
        else
        {
            var fileName = $"Món ăn phân chia.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
