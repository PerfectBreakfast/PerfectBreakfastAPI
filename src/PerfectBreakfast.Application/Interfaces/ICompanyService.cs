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
    public Task<OperationResult<CompanyResponse>> UpdateCompany(Guid id, UpdateCompanyRequest companyRequest);
    public Task<OperationResult<Pagination<CompanyResponsePaging>>> GetCompanyPaginationAsync(string? searchTerm, int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<CompanyResponse>> Delete(Guid id);
    public Task<OperationResult<List<UserResponse>>> GetUsersByCompanyId(Guid id);
    public Task<OperationResult<Pagination<CompanyResponsePaging>>> GetCompanyByPartner(string? searchTerm, int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<Pagination<CompanyResponsePaging>>> GetCompanyByDelivery(string? searchTerm, int pageIndex = 0, int pageSize = 10);
}