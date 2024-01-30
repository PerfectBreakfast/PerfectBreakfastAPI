using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PartnerModels.Request;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/partners")]
public class PartnerController : BaseController
{
    private readonly IPartnerService _partnerService;
    
    public PartnerController(IPartnerService partnerService)
    {
        _partnerService = partnerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetManagementUnits()
    {
        var response = await _partnerService.GetManagementUnits();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateManagementUnit(CreateManagementUnitRequest requestModel)
    {
        var response = await _partnerService.CreateManagementUnit(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManagementUnit(Guid id, UpdateManagementUnitRequest requestModel)
    {
        var response = await _partnerService.UpdateManagementUnit(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveManagementUnit(Guid id)
    {
        var response = await _partnerService.RemoveManagementUnit(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetManagementUnitId(Guid id)
    {
        var response = await _partnerService.GetManagementUnitId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetManagemnetUnitPagination(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var response = await _partnerService.GetManagementUnitPaginationAsync(searchTerm,pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}