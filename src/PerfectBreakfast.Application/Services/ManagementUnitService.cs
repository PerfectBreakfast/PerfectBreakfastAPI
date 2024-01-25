using System.Linq.Expressions;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Request;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services;

public class ManagementUnitService : IManagementUnitService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ManagementUnitService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
        
    public async Task<OperationResult<List<ManagementUnitResponseModel>>> GetManagementUnits()
    {
        var result = new OperationResult<List<ManagementUnitResponseModel>>();
        try
        {
            var managementUnits = await _unitOfWork.ManagementUnitRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<ManagementUnitResponseModel>>(managementUnits);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<ManagementUnitDetailResponse>> GetManagementUnitId(Guid id)
    {
        var result = new OperationResult<ManagementUnitDetailResponse>();
        try
        {
            var managementUnit = await _unitOfWork.ManagementUnitRepository.GetManagementUintDetail(id);
            if (managementUnit == null)
            {
                result.AddUnknownError("Id does not exist");
                return result;
            }

            var suppliers = managementUnit.SupplyAssignments.Select(o => o.Supplier).ToList();
            var mana = _mapper.Map<ManagementUnitDetailResponse>(managementUnit);
            mana.SupplierDTO = _mapper.Map<List<SupplierDTO>>(suppliers);

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

    public async Task<OperationResult<List<RoleResponse>>> GetRoleByManagementUnit()
    {
        var result = new OperationResult<List<RoleResponse>>();
        try
        {
            var roles = await _unitOfWork.RoleRepository.FindAll(x => x.UnitCode == UnitCode.ManagementUnit).ToListAsync();
            result.Payload = _mapper.Map<List<RoleResponse>>(roles);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<ManagementUnitResponseModel>> CreateManagementUnit(CreateManagementUnitRequest requestModel)
    {
        var result = new OperationResult<ManagementUnitResponseModel>();
        try
        {
            // map model to Entity
            var managementUnit = _mapper.Map<ManagementUnit>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.ManagementUnitRepository.AddAsync(managementUnit);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<ManagementUnitResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<ManagementUnitResponseModel>> UpdateManagementUnit(Guid managementUnitId, UpdateManagementUnitRequest requestModel)
    {
        var result = new OperationResult<ManagementUnitResponseModel>();
        try
        {
            // find supplier by ID
            var managementUnit = await _unitOfWork.ManagementUnitRepository.GetByIdAsync(managementUnitId);
            // map from requestModel => supplier
            _mapper.Map(requestModel, managementUnit);
            // update
            _unitOfWork.ManagementUnitRepository.Update(managementUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<ManagementUnitResponseModel>(managementUnit);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<ManagementUnitResponseModel>> RemoveManagementUnit(Guid managementUnitIdId)
    {
        var result = new OperationResult<ManagementUnitResponseModel>();
        try
        {
            // find supplier by ID
            var managementUnit = await _unitOfWork.ManagementUnitRepository.GetByIdAsync(managementUnitIdId);
            // Remove
            var entity = _unitOfWork.ManagementUnitRepository.Remove(managementUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<ManagementUnitResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Pagination<ManagementUnitResponseModel>>> GetManagementUnitPaginationAsync(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<ManagementUnitResponseModel>>();
        try
        {
            // xác định các thuộc tính include và theninclude 
            var UserInclude = new IncludeInfo<ManagementUnit>
            {
                NavigationProperty = c => c.Users
            };
            var partnerPages = await _unitOfWork.ManagementUnitRepository.ToPagination(pageIndex, pageSize,null,UserInclude);
            var managementUnitResponses = partnerPages.Items.Select(mu => 
                new ManagementUnitResponseModel(mu.Id,mu.Name, mu.Address, mu.CommissionRate, mu.Longitude, mu.Latitude, mu.Users.Count)).ToList();
            
            result.Payload = new Pagination<ManagementUnitResponseModel>
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
}