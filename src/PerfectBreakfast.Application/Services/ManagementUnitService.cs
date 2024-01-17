using Mapster;
using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Request;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Domain.Entities;

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

    public async Task<OperationResult<ManagementUnitResponseModel>> GetManagementUnitId(Guid Id)
    {
        var result = new OperationResult<ManagementUnitResponseModel>();
        try
        {
            var managementUnit = await _unitOfWork.ManagementUnitRepository.GetManagementUintDetail(Id);
            if (managementUnit == null)
            {
                result.AddUnknownError("Id does not exist");
                return result;
            }

            var suppliers = managementUnit.SupplyAssignments.Select(o => o.Supplier).ToList();
            var mana = _mapper.Map<ManagementUnitResponseModel>(managementUnit);
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
}