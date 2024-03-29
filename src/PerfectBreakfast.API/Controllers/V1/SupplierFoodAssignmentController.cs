﻿using Microsoft.AspNetCore.Authorization;
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
    private readonly IExportExcelService _exportExcelService;

    public SupplierFoodAssignmentController(ISupplierFoodAssignmentService supplierFoodAssignmentService, IExportExcelService exportExcelService)
    {
        _supplierFoodAssignmentService = supplierFoodAssignmentService;
        _exportExcelService = exportExcelService;
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
    /// API For Supplier Admin - Supplier download
    /// </summary>
    /// <param name="bookingDate"></param>
    /// <returns></returns>
    [HttpGet("download-excel")]
    [Authorize(Roles = "SUPPLIER ADMIN")]
    public async Task<IActionResult> DownloadSupplierFoodAssignment(DateOnly bookingDate)
    {
        
        var response = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentsByBookingDate(bookingDate);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        var content = _exportExcelService.DownloadSupplierFoodAssignmentForSupplier(response.Payload[0]);
        if (content == null)
        {
            return NotFound("Some thing wrong");
        }
        else
        {
            var fileName = $"Món ăn phân chia cho ngày {bookingDate.ToString("dd/mmm/yyyy")}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }

    /// <summary>
    /// API For Super Admin - Super Admin download theo ngày giao hàng
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    [HttpGet("super-admin/download-excel")]
    public async Task<IActionResult> DownloadSupplierFoodAssignmentBySuperAdmin(DateOnly fromDate, DateOnly toDate)
    {
        
        var response = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentsForSuperAdmin(fromDate, toDate);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        var content = _exportExcelService.DownloadSupplierFoodAssignmentForSuperAdmin(response.Payload);
        if (content == null)
        {
            return NotFound("Some thing wrong");
        }
        else
        {
            var fileName = $"Món ăn phân chia từ {fromDate} đến {toDate}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
