using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Request;
using PerfectBreakfast.Application.Utils;

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

    /// <summary>
    /// API for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var response = await _companyService.GetCompany(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin, Partner Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}/users")]
    [Authorize(Roles = "SUPER ADMIN, PARTNER ADMIN")]
    public async Task<IActionResult> GetUsersByCompany(Guid id)
    {
        var response = await _companyService.GetUsersByCompanyId(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin
    /// </summary>
    /// <param name="companyRequest"></param>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreateCompany(CompanyRequest companyRequest)
    {
        var response = await _companyService.CreateCompany(companyRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <param name="companyRequest"></param>
    /// <returns></returns>
    [HttpPut("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> UpdateCompany(Guid id, UpdateCompanyRequest companyRequest)
    {
        var response = await _companyService.UpdateCompany(id, companyRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Supper Admin
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> GetCompanyPagination(string? searchTerm, int pageIndex = 0, int pageSize = 10)
    {
        var response = await _companyService.GetCompanyPaginationAsync(searchTerm, pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// API for Super Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _companyService.DeleteCompany(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Partner Admin
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination/partner"), Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
    public async Task<IActionResult> GetCompaniesByPartner(string? searchTerm, int pageIndex = 0, int pageSize = 10)
    {
        var response = await _companyService.GetCompanyByPartner(searchTerm, pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    /// <summary>
    /// API For Delivery Admin
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination/delivery"), Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
    public async Task<IActionResult> GetCompaniesByDelivery(string? searchTerm, int pageIndex = 0, int pageSize = 10)
    {
        var response = await _companyService.GetCompanyByDelivery(searchTerm, pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}