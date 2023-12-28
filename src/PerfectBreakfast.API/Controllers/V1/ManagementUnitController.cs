using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/managementunits")]
public class ManagementUnitController : BaseController
{
    private readonly IManagementUnitService _managementUnitService;

    public ManagementUnitController(IManagementUnitService managementUnitService)
    {
        _managementUnitService = managementUnitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetManagementUnits()
    {
        var response = await _managementUnitService.GetManagementUnits();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}