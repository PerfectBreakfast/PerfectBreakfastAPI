using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;

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
    public async Task<IActionResult> Get()
    {
        var response = await _companyService.GetAllCompanies();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}