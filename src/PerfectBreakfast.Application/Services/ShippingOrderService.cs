using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
using PerfectBreakfast.Domain.Entities;
namespace PerfectBreakfast.Application.Services;

public class ShippingOrderService : IShippingOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;

    public ShippingOrderService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
    }

    public async Task<OperationResult<ShippingOrderRespone>> CreateShippingOrder(CreateShippingOrderRequest requestModel)
    {
        var result = new OperationResult<ShippingOrderRespone>();
        try
        {
            // map model to Entity
            var shippingOrder = _mapper.Map<ShippingOrder>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.ShippingOrderRepository.AddAsync(shippingOrder);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<ShippingOrderRespone>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}
