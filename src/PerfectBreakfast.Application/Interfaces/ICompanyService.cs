using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CompanyModels.Request;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface ICompanyService
{
    public Task<OperationResult<List<CompanyResponse>>> GetAllCompanies();
    public Task<OperationResult<CompanyResponse>> GetCompany(Guid companyId);
    public Task<OperationResult<CompanyResponse>> CreateCompany(CompanyRequest companyRequest);
    public Task<OperationResult<CompanyResponse>> DeleteCompany(Guid id);
    public Task<OperationResult<CompanyResponse>> UpdateCompany(Guid Id, CompanyRequest companyRequest);
    public Task<OperationResult<Pagination<CompanyResponse>>> GetCompanyPaginationAsync(int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<CompanyResponse>> Delete(Guid id);
    public Task<OperationResult<List<UserResponse>>> GetUsersByCompanyId(Guid id);
}