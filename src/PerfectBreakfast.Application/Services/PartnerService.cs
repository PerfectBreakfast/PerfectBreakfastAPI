using System.Linq.Expressions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
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
            var partners = await _unitOfWork.PartnerRepository.FindAll(p => !p.IsDeleted).ToListAsync();
            result.Payload = _mapper.Map<List<PartnerResponseModel>>(partners);
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
                result.AddError(ErrorCode.NotFound,"Id does not exist");
                return result;
            }
            var mana = _mapper.Map<PartnerDetailResponse>(partner);
            
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
            _unitOfWork.PartnerRepository.SoftRemove(managementUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<PartnerResponseModel>(managementUnit);
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
                NavigationProperty = c => c.Users,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((User)sp).UserRoles,
                    sp => ((UserRole)sp).Role
                }
            };
            
            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<Partner, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm) 
                ? (x => !x.IsDeleted) 
                : (x => x.Name.ToLower().Contains(searchTerm.ToLower()) && !x.IsDeleted);
            
            var partnerPages = await _unitOfWork.PartnerRepository.ToPagination(pageIndex, pageSize,searchPredicate,userInclude);
            var managementUnitResponses = new List<PartnerResponseModel>();
            foreach (var mr in partnerPages.Items)
            {
                var users = mr.Users.Where(u => u.UserRoles.Any(ur => ur.Role.Name == ConstantRole.PARTNER_ADMIN))
                    .Select(u => u.Name)
                    .ToList();

                var managementUnitResponse = new PartnerResponseModel(
                    mr.Id,
                    mr.Name,
                    mr.Address,
                    mr.PhoneNumber,
                    mr.CommissionRate,
                    mr.Longitude,
                    mr.Latitude,
                    users, // Danh sách người dùng là admin
                    null,
                    null,
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

    public async Task<OperationResult<List<PartnerResponseModel>>> AssignPartnerToSupplier(Guid supplierId)
    {
        var result = new OperationResult<List<PartnerResponseModel>>();
        try
        {
            var partners = await _unitOfWork.PartnerRepository
                .FindAll(p => !p.IsDeleted && p.SupplyAssignments.All(s => s.SupplierId != supplierId))
                .ToListAsync();
            result.Payload = _mapper.Map<List<PartnerResponseModel>>(partners);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}