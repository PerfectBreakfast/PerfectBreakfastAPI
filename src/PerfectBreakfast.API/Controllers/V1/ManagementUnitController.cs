using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Request;

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
    
    [HttpPost]
    public async Task<IActionResult> CreateManagementUnit(CreateManagementUnitRequest requestModel)
    {
        var response = await _managementUnitService.CreateManagementUnit(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManagementUnit(Guid id, UpdateManagementUnitRequest requestModel)
    {
        var response = await _managementUnitService.UpdateManagementUnit(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveManagementUnit(Guid id)
    {
        var response = await _managementUnitService.RemoveManagementUnit(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}