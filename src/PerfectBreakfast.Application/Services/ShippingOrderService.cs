using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DailyOrder.Response;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

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

    // get all shipper with dalilyOder Detail
    public async Task<OperationResult<List<ShippingOrderDTO>>> GetAllShippingOrdersWithDetails()
    {
        var result = new OperationResult<List<ShippingOrderDTO>>();
        try
        {
            var shippingOrders = await _unitOfWork.ShippingOrderRepository
                .GetAllWithDailyOrdersAsync(); // Assuming this correctly fetches ShippingOrder including DailyOrder

            var shippingOrderDTOs = shippingOrders.Select(so => new ShippingOrderDTO
            {
                ShipperId = so.ShipperId,
                DailyOrder = _mapper.Map<DailyOrderResponse>(so.DailyOrder) // Direct mapping without Select
            }).ToList();

            result.Payload = shippingOrderDTOs;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<ShippingOrderResponse>>> CreateShippingOrder(CreateShippingOrderRequest requestModel)
    {
        var result = new OperationResult<List<ShippingOrderResponse>>();
        var responses = new List<ShippingOrderResponse>();
        try
        {
            if (requestModel.DailyOrderId.HasValue)
            {
                var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync(requestModel.DailyOrderId.Value);
                if (dailyOrder == null)
                {
                    result.AddError(ErrorCode.NotFound, "The DailyOrder with the provided ID was not found.");
                    return result;
                }
            }
            
            // Validate ShipperIds
            foreach (var shipperId in requestModel.ShipperIds)
            {
                var shipper = await _unitOfWork.UserManager.FindByIdAsync(shipperId.ToString());
                if (shipper == null)
                {
                    result.AddError(ErrorCode.NotFound, $"The user with the provided ID {shipperId} was not found.");
                    continue; // Consider strategy for partial failure
                }
                if (!(await _unitOfWork.UserManager.IsInRoleAsync(shipper, ConstantRole.DELIVERY_STAFF)))
                {
                    result.AddError(ErrorCode.BadRequest, $"The user {shipperId} is not a DELIVERY STAFF.");
                    continue; // Consider strategy for partial failure
                }
                
                // Check for duplicate shipping order
                if (requestModel.DailyOrderId.HasValue && requestModel.ShipperIds.Any())
                {
                    bool exists = await _unitOfWork.ShippingOrderRepository.ExistsWithDailyOrderAndShippers(
                        requestModel.DailyOrderId.Value, requestModel.ShipperIds);

                    if (exists)
                    {
                        result.AddError(ErrorCode.BadRequest, "A shipping order with the same DailyOrderId and one of the ShipperIds already exists.");
                        return result;
                    }
                }

                var shippingOrder = new ShippingOrder
                {
                    ShipperId = shipperId,
                    DailyOrderId = requestModel.DailyOrderId,
                    Status = ShippingStatus.Pending
                };
                
                //list.Add(shippingOrder);
                // Add to DB
                await _unitOfWork.ShippingOrderRepository.AddAsync(shippingOrder);
                // map model to response
                
                var responseList = _mapper.Map<ShippingOrderResponse>(shippingOrder);
                
                responses.Add(responseList);
               

            }
            await _unitOfWork.SaveChangeAsync();
            result.Payload = responses;
            return result;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<List<ShippingOrderHistoryForShipperResponse>>> GetShippingOrderByDeliveryStaff()
    {
        var result = new OperationResult<List<ShippingOrderHistoryForShipperResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            //var shippingOrders = await _unitOfWork.ShippingOrderRepository.FindAll(so => so.ShipperId == userId).ToListAsync();
            var shippingOrders = await _unitOfWork.ShippingOrderRepository.GetShippingOrderByShipperId(userId);
            result.Payload = _mapper.Map<List<ShippingOrderHistoryForShipperResponse>>(shippingOrders);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}
