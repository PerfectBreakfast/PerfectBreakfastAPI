using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

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
    /// API For Management Unit Admin
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateSupplierFoodAssignment(List<SupplierFoodAssignmentRequest> supplierFoodAssignmentRequest)
    {
        var response = await _supplierFoodAssignmentService.CreateSupplierFoodAssignment(supplierFoodAssignmentRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}
