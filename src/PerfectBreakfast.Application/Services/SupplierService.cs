using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SupplierService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<OperationResult<List<SupplierResponse>>> GetSuppliers()
    {
        var result = new OperationResult<List<SupplierResponse>>();
        try
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<SupplierResponse>>(suppliers);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierResponse>> GetSupplierId(Guid Id)
    {
        var result = new OperationResult<SupplierResponse>();
        try
        {
            var supp = await _unitOfWork.SupplierRepository.GetSupplierUintDetail(Id);
            if (supp == null)
            {
                result.AddUnknownError("Id does not exist");
                return result;
            }

            var managementUnit = supp.SupplyAssignments.Select(o => o.ManagementUnit).ToList();
            
            var supplier = _mapper.Map<SupplierResponse>(supp);
            
            supplier.ManagementUnitDtos = _mapper.Map<List<ManagementUnitDTO>>(managementUnit);

            result.Payload = supplier;
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

    public async Task<OperationResult<List<RoleResponse>>> GetRoleBySupplier()
    {
        var result = new OperationResult<List<RoleResponse>>();
        try
        {
            var roles = await _unitOfWork.RoleRepository.FindAll(x => x.UnitCode == UnitCode.Supplier).ToListAsync();
            result.Payload = _mapper.Map<List<RoleResponse>>(roles);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierResponse>> CreateSupplier(CreateSupplierRequestModel requestModel)
    {
        var result = new OperationResult<SupplierResponse>();
        try
        {
            // map model to Entity
            var supplier = _mapper.Map<Supplier>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.SupplierRepository.AddAsync(supplier);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<SupplierResponse>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierResponse>> UpdateSupplier(Guid supplierId,UpdateSupplierRequestModel requestModel)
    {
        var result = new OperationResult<SupplierResponse>();
        try
        {
            // find supplier by ID
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(supplierId);
            // map from requestModel => supplier
            _mapper.Map(requestModel, supplier);
            // update
            _unitOfWork.SupplierRepository.Update(supplier);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<SupplierResponse>(supplier);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Pagination<SupplierResponse>>> GetPaginationAsync(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<SupplierResponse>>();
        try
        {
            var pagination = await _unitOfWork.SupplierRepository.ToPagination(pageIndex,pageSize);
            result.Payload = _mapper.Map<Pagination<SupplierResponse>>(pagination);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierResponse>> RemoveSupplier(Guid supplierId)
    {
        var result = new OperationResult<SupplierResponse>();
        try
        {
            // find supplier by ID
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(supplierId);
            // Remove
            var entity = _unitOfWork.SupplierRepository.Remove(supplier);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<SupplierResponse>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}