using System.Linq.Expressions;
using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.PartnerModels.Request;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class PartnerService : IPartnerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PartnerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<List<PartnerResponseModel>>> GetPartners()
    {
        var result = new OperationResult<List<PartnerResponseModel>>();
        try
        {
            var managementUnits = await _unitOfWork.PartnerRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<PartnerResponseModel>>(managementUnits);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<PartnerDetailResponse>> GetPartnerId(Guid id)
    {
        var result = new OperationResult<PartnerDetailResponse>();
        try
        {
            var partner = await _unitOfWork.PartnerRepository.GetPartnerDetail(id);
            if (partner == null)
            {
                result.AddUnknownError("Id does not exist");
                return result;
            }

            var suppliers = partner.SupplyAssignments.Select(o => o.Supplier).ToList();
            var mana = _mapper.Map<PartnerDetailResponse>(partner);
            mana.SupplierDTO = _mapper.Map<List<SupplierDTO>>(suppliers);
            mana.Companies = _mapper.Map<List<CompanyResponsePaging>>(partner.Companies);
            result.Payload = mana;
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound, e.Message);
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }

    public async Task<OperationResult<PartnerResponseModel>> CreatePartner(CreatePartnerRequest requestModel)
    {
        var result = new OperationResult<PartnerResponseModel>();
        try
        {
            // map model to Entity
            var managementUnit = _mapper.Map<Partner>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.PartnerRepository.AddAsync(managementUnit);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<PartnerResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<PartnerResponseModel>> UpdatePartner(Guid managementUnitId, UpdatePartnerRequest requestModel)
    {
        var result = new OperationResult<PartnerResponseModel>();
        try
        {
            // find supplier by ID
            var managementUnit = await _unitOfWork.PartnerRepository.GetByIdAsync(managementUnitId);
            // map from requestModel => supplier
            //_mapper.Map(requestModel, managementUnit);
            managementUnit.Name = requestModel.Name ?? managementUnit.Name;
            managementUnit.Address = requestModel.Address ?? managementUnit.Address;
            managementUnit.PhoneNumber = requestModel.PhoneNumber ?? managementUnit.PhoneNumber;
            managementUnit.CommissionRate = requestModel.CommissionRate ?? managementUnit.CommissionRate;
            // update
            _unitOfWork.PartnerRepository.Update(managementUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<PartnerResponseModel>(managementUnit);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<PartnerResponseModel>> RemovePartner(Guid managementUnitIdId)
    {
        var result = new OperationResult<PartnerResponseModel>();
        try
        {
            // find supplier by ID
            var managementUnit = await _unitOfWork.PartnerRepository.GetByIdAsync(managementUnitIdId);
            // Remove
            var entity = _unitOfWork.PartnerRepository.Remove(managementUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<PartnerResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Pagination<PartnerResponseModel>>> GetPartnerPaginationAsync(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<PartnerResponseModel>>();
        try
        {
            // xác định các thuộc tính include và theninclude 
            var userInclude = new IncludeInfo<Partner>
            {
                NavigationProperty = c => c.Users
            };
            var supplierInclude = new IncludeInfo<Partner>
            {
                NavigationProperty = x => x.SupplyAssignments,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((SupplyAssignment)sp).Supplier
                }
            };
            var companyInclude = new IncludeInfo<Partner>
            {
                NavigationProperty = c => c.Companies
            };
            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<Partner, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm) 
                ? null 
                : (x => x.Name.ToLower().Contains(searchTerm.ToLower()));
            
            var partnerPages = await _unitOfWork.PartnerRepository.ToPagination(pageIndex, pageSize,searchPredicate,userInclude,supplierInclude,companyInclude);
            var managementUnitResponses = new List<PartnerResponseModel>();
            foreach (var mr in partnerPages.Items)
            {
                var adminUserNames = new List<string>();

                foreach (var user in mr.Users)   // lấy ra danh sách user có role là Supplier Admin
                {
                    if (await CheckIfUserIsAdmin(user))
                    {
                        adminUserNames.Add(user.Name);
                    }
                }

                var managementUnitResponse = new PartnerResponseModel(
                    mr.Id,
                    mr.Name,
                    mr.Address,
                    mr.PhoneNumber,
                    mr.CommissionRate,
                    mr.Longitude,
                    mr.Latitude,
                    adminUserNames, // Danh sách người dùng là admin
                    mr.Companies.Select(c => c.Name).ToList(),
                    mr.SupplyAssignments.Select(sa => sa.Supplier.Name).ToList(),
                    mr.Users.Count);

                managementUnitResponses.Add(managementUnitResponse);
            }
            
            result.Payload = new Pagination<PartnerResponseModel>
            {
                PageIndex = partnerPages.PageIndex,
                PageSize = partnerPages.PageSize,
                TotalItemsCount = partnerPages.TotalItemsCount,
                Items = managementUnitResponses
            };
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
    
    private async Task<bool> CheckIfUserIsAdmin(User user)
    {
        var roles = await _unitOfWork.UserManager.GetRolesAsync(user);
        return roles.Contains(ConstantRole.PARTNER_ADMIN);
    }
}