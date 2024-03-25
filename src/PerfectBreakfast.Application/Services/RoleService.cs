using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<List<RoleResponse>>> GetManagementRole()
    {
        var result = new OperationResult<List<RoleResponse>>();
        try
        {
            var roles = await _unitOfWork.RoleRepository.GetAllAsync();
            var filteredRoles = roles.Where(role => role.Name != ConstantRole.CUSTOMER).ToList();
            result.Payload = _mapper.Map<List<RoleResponse>>(filteredRoles);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<RoleResponse>> GetRoleById(Guid id)
    {
        var result = new OperationResult<RoleResponse>();
        try
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            result.Payload = _mapper.Map<RoleResponse>(role);
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound, e.Message);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<RoleResponse>>> GetRoleByUnitId(Guid unitId)
    {
        var result = new OperationResult<List<RoleResponse>>();
        try
        {
            // Kiểm tra ManagementUnit
            var managementUnit = await _unitOfWork.PartnerRepository.GetPartnerById(unitId);
            if (managementUnit != null)
            {
                // Lấy roles từ Partner
                var roles = await _unitOfWork.RoleRepository.FindAll(x => x.UnitCode == UnitCode.Partner).ToListAsync();
                result.Payload = _mapper.Map<List<RoleResponse>>(roles);
                return result;
            }

            // Kiểm tra DeliveryUnit
            var delivery = await _unitOfWork.DeliveryRepository.GetDeliveryById(unitId);
            if (delivery != null)
            {
                // Lấy roles từ DeliveryUnit
                var roles = await _unitOfWork.RoleRepository.FindAll(x => x.UnitCode == UnitCode.Delivery)
                    .ToListAsync();
                result.Payload = _mapper.Map<List<RoleResponse>>(roles);
                return result;
            }

            // Kiểm tra Supplier
            var supplier = await _unitOfWork.SupplierRepository.GetSupplierById(unitId);
            if (supplier != null)
            {
                // Lấy roles từ Supplier
                var roles = await _unitOfWork.RoleRepository.FindAll(x => x.UnitCode == UnitCode.Supplier)
                    .ToListAsync();
                result.Payload = _mapper.Map<List<RoleResponse>>(roles);
                return result;
            }

            result.AddError(ErrorCode.NotFound, $"Not found by Id : {unitId}");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> CreateRole(CreatRoleRequest requestModel)
    {
        var result = new OperationResult<bool>();
        try
        {
            var role = new Role { Name = requestModel.Name };
            var identityResult = await _unitOfWork.RoleManager.CreateAsync(role);
            if (!identityResult.Succeeded)
            {
                result.AddError(ErrorCode.ServerError, identityResult.Errors.Select(x => x.Description).ToString());
                return result;
            }

            result.Payload = identityResult.Succeeded;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> UpdateRole(Guid roleId, UpdateRolerequest requestModel)
    {
        var result = new OperationResult<bool>();
        try
        {
            // find supplier by ID
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
            _mapper.Map(requestModel, role);
            _unitOfWork.RoleRepository.Update(role);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            result.Payload = isSuccess;
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound, e.Message);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> RemoveRole(Guid roleId)
    {
        var result = new OperationResult<bool>();
        try
        {
            // find supplier by ID
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
            _unitOfWork.RoleRepository.Remove(role);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            result.Payload = isSuccess;
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound, e.Message);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}