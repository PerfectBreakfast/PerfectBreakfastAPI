using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class DeliveryUnitService : IDeliveryUnitService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DeliveryUnitService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<OperationResult<List<DeliveryUnitResponseModel>>> GetDeliveries()
    {
        var result = new OperationResult<List<DeliveryUnitResponseModel>>();
        try
        {
            var deliveryUnits = await _unitOfWork.DeliveryUnitRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<DeliveryUnitResponseModel>>(deliveryUnits);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryUnitResponseModel>> CreateDelivery(CreateDeliveryUnitRequest requestModel)
    {
        var result = new OperationResult<DeliveryUnitResponseModel>();
        try
        {
            // map model to Entity
            var deliveryUnit = _mapper.Map<DeliveryUnit>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.DeliveryUnitRepository.AddAsync(deliveryUnit);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<DeliveryUnitResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryUnitResponseModel>> UpdateDelivery(Guid deliveryId, UpdateDeliveryUnitRequest requestModel)
    {
        var result = new OperationResult<DeliveryUnitResponseModel>();
        try
        {
            // find supplier by ID
            var deliveryUnit = await _unitOfWork.DeliveryUnitRepository.GetByIdAsync(deliveryId);
            // map from requestModel => supplier
            _mapper.Map(requestModel, deliveryUnit);
            // update
            _unitOfWork.DeliveryUnitRepository.Update(deliveryUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<DeliveryUnitResponseModel>(deliveryUnit);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryUnitResponseModel>> RemoveDelivery(Guid deliveryId)
    {
        var result = new OperationResult<DeliveryUnitResponseModel>();
        try
        {
            // find supplier by ID
            var deliveryUnit = await _unitOfWork.DeliveryUnitRepository.GetByIdAsync(deliveryId);
            // Remove
            var entity = _unitOfWork.DeliveryUnitRepository.Remove(deliveryUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<DeliveryUnitResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryUnitResponseModel>> GetDeliveryId(Guid deliveryId)
    {
        var result = new OperationResult<DeliveryUnitResponseModel>();
            try
            {
                var deliveryUnit = await _unitOfWork.DeliveryUnitRepository.GetByIdAsync(deliveryId);
                result.Payload = _mapper.Map<DeliveryUnitResponseModel>(deliveryUnit);
            }
            catch (NotFoundIdException e)
            {
                result.AddError(ErrorCode.NotFound,e.Message);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
    }
}