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
    public async Task<IActionResult> GetPartners()
    {
        var response = await _partnerService.GetPartners();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePartner(CreatePartnerRequest requestModel)
    {
        var response = await _partnerService.CreatePartner(requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePartner(Guid id, UpdatePartnerRequest requestModel)
    {
        var response = await _partnerService.UpdatePartner(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemovePartner(Guid id)
    {
        var response = await _partnerService.RemovePartner(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPartnerId(Guid id)
    {
        var response = await _partnerService.GetPartnerId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetPartnerPagination(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var response = await _partnerService.GetPartnerPaginationAsync(searchTerm,pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}