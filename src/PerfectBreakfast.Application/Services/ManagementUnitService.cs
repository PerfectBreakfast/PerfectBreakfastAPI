using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;

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
}