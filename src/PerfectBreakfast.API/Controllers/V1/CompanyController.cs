using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Request;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/companies")]
public class CompanyController : BaseController
{
    private readonly ICompanyService _companyService;
    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var response = await _companyService.GetAllCompanies();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var response = await _companyService.GetCompany(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany(CompanyRequest companyRequest)
    {
        var response = await _companyService.CreateCompany(companyRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCompany(Guid id, UpdateCompanyRequest updateCompanyRequest)
    {
        updateCompanyRequest.Id = id;
        var response = await _companyService.UpdateCompany(updateCompanyRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPut("delete/{id}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        var response = await _companyService.DeleteCompany(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}