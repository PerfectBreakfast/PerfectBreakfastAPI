using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
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
    
    public async Task<OperationResult<bool>> CreateShippingOrder(
        CreateShippingOrderRequest requestModel)
    {
        var result = new OperationResult<bool>();
        var list = new List<ShippingOrder>();
        try
        {
            var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync(requestModel.DailyOrderId);
            if (dailyOrder.Status != DailyOrderStatus.Processing && dailyOrder.Status != DailyOrderStatus.Cooking && dailyOrder.Status !=DailyOrderStatus.Waiting && dailyOrder.Status !=DailyOrderStatus.Divided)
            {
                result.AddError(ErrorCode.BadRequest,
                    $"Trạng thái đơn hàng không đúng {dailyOrder.Status.ToString()}");
                return result;
            }

            // Check for duplicate shipping order
            var exists = await _unitOfWork.ShippingOrderRepository.ExistsWithDailyOrderAndShippers(
                requestModel.DailyOrderId, requestModel.ShipperIds);
            if (exists)
            {
                result.AddError(ErrorCode.BadRequest,
                    "A shipping order with the same DailyOrderId and one of the ShipperIds already exists.");
                return result;
            }

            list.AddRange(requestModel.ShipperIds.Select(shipperId => new ShippingOrder
            {
                ShipperId = shipperId,
                DailyOrderId = requestModel.DailyOrderId,
                Status = ShippingStatus.Pending
            }));
            await _unitOfWork.ShippingOrderRepository.AddRangeAsync(list);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            result.Payload = isSuccess;
        }
        catch (NotFoundIdException)
        {
            result.AddError(ErrorCode.NotFound, "ID was not found.");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<List<ShippingOrderForShipperResponse>>> GetShippingOrderTodayByDeliveryStaff(DateTime time)
    {
        var result = new OperationResult<List<ShippingOrderForShipperResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            // Hàm này còn đang sửa tiêp cho nagyf mai 
            var date = DateOnly.FromDateTime(time);
            var shippingOrders = await _unitOfWork.ShippingOrderRepository.GetShippingOrderTodayByShipperIdAndDate(userId,date);
            
            result.Payload = _mapper.Map<List<ShippingOrderForShipperResponse>>(shippingOrders);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<bool>> ConfirmShippingOrderByShipper(Guid dailyOrderId)
    {
        var result = new OperationResult<bool>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var shippingOrders = await _unitOfWork.ShippingOrderRepository
                .GetShippingOrderByDailyOrderId(dailyOrderId);
            // check đơn hàng có phải phân công đúng của shipper không ?
            if (!shippingOrders.Any() || shippingOrders.All(order => order.ShipperId != userId))
            {
                result.AddError(ErrorCode.BadRequest, "Đơn hàng này không phải nhiệm vụ của bạn");
                return result;
            }
            
            // check xem xem tổng đơn(dailyOrder) đã ở trạng thái sẵn sàng chờ đến lấy đi giao hay chưa ? 
            var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync(dailyOrderId);
            if (dailyOrder is not { Status: DailyOrderStatus.Waiting })
            {
                switch (dailyOrder.Status)
                {
                    case DailyOrderStatus.Complete:
                        result.AddError(ErrorCode.BadRequest, "Đơn đã hoàn thành rồi nhé");
                        return result;
                    case DailyOrderStatus.Cooking:
                        result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá trình nấu");
                        return result;
                    case DailyOrderStatus.Initial:
                        result.AddError(ErrorCode.BadRequest, "Đơn chưa sẵn sàng");
                        return result;
                    case DailyOrderStatus.Processing:
                        result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá xử lý");
                        return result;
                    case DailyOrderStatus.Divided:
                        result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá xử lý");
                        return result;
                    case DailyOrderStatus.NoOrders:
                        result.AddError(ErrorCode.BadRequest, "Đơn này không có");
                        return result;
                    case DailyOrderStatus.Delivering:
                        result.AddError(ErrorCode.BadRequest, "Đơn đã đang trong quá trình giao");
                        return result;
                    case DailyOrderStatus.Waiting:
                        break;
                    default:
                        result.AddError(ErrorCode.BadRequest, "Không thể xác nhận đơn hàng lúc này");
                        return result;
                }
            }
            
            if (shippingOrders.All(s => s.Status == ShippingStatus.Pending))
            {
                foreach (var shippingOrder in shippingOrders)
                {
                    shippingOrder.Status = ShippingStatus.Confirm;
                } 
            }
            else
            {
                result.AddError(ErrorCode.ServerError, "Có 1 đơn vận chuyển không phải trong trạng thái chờ xác nhận");
                return result;
            }
            _unitOfWork.ShippingOrderRepository.UpdateRange(shippingOrders);
            dailyOrder.Status = DailyOrderStatus.Delivering;
            _unitOfWork.DailyOrderRepository.Update(dailyOrder);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "lỗi trong quá trình lưu xuống db");
                return result;
            }
            result.Payload = isSuccess;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
    
    public async Task<OperationResult<List<UserResponse>>> GetSDeliveryStaffByDailyOrder(Guid dailyOrderId)
    {
        var result = new OperationResult<List<UserResponse>>();
        try
        {
            var users = await _unitOfWork.ShippingOrderRepository.GetStaffByDailyOrder(dailyOrderId);
            result.Payload = _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}