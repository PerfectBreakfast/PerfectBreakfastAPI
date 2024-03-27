using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.PartnerModels.Request;
using PerfectBreakfast.Application.Models.PartnerModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IPartnerService
{
    public Task<OperationResult<List<PartnerResponseModel>>> GetPartners();
    public Task<OperationResult<PartnerDetailResponse>> GetPartnerId(Guid id);
    public Task<OperationResult<PartnerResponseModel>> CreatePartner(CreatePartnerRequest requestModel);
    public Task<OperationResult<PartnerResponseModel>> UpdatePartner(Guid managementUnitId, UpdatePartnerRequest requestModel);
    public Task<OperationResult<PartnerResponseModel>> RemovePartner(Guid managementUnitIdId);
    public Task<OperationResult<Pagination<PartnerResponseModel>>> GetPartnerPaginationAsync(string? searchTerm,int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<List<PartnerResponseModel>>> AssignPartnerToSupplier(Guid supplierId);
}