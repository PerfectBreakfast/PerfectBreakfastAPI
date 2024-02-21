using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Request;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class SupplyAssigmentService : ISupplyAssigmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SupplyAssigmentService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<OperationResult<List<SupplyAssigmentResponse>>> GetSupplyAssigment()
    {
        var result = new OperationResult<List<SupplyAssigmentResponse>>();
        try
        {
            var suppliers = await _unitOfWork.SupplyAssigmentRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<SupplyAssigmentResponse>>(suppliers);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplyAssigmentResponse>> CreateSupplyAssigment(CreateSupplyAssigment requestModel)
    {
        var result = new OperationResult<SupplyAssigmentResponse>();
        try
        {
            if (requestModel.PartnerId.HasValue && requestModel.SupplierId.HasValue)
            {
                bool isDuplicate = await _unitOfWork.SupplyAssigmentRepository
                    .IsDuplicateAssignment(requestModel.PartnerId.Value, requestModel.SupplierId.Value);
                if (isDuplicate)
                {
                    result.AddError(ErrorCode.BadRequest, "A supply assignment with the same PartnerId and SupplierId already exists.");
                    return result;
                }
            }
            // map model to Entity
            var supplier = _mapper.Map<SupplyAssignment>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.SupplyAssigmentRepository.AddAsync(supplier);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<SupplyAssigmentResponse>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    
}