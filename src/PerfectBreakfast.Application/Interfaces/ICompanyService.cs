using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface ICompanyService
{
    public Task<OperationResult<List<CompanyResponse>>> GetAllCompanies();
}