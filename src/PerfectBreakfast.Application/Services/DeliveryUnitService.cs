using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

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
}