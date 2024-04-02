using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;


namespace PerfectBreakfast.Application.Interfaces;

public interface IShippingOrderService
{
    public Task<OperationResult<bool>> CreateShippingOrder(CreateShippingOrderRequest requestModel);
    public Task<OperationResult<List<ShippingOrderForShipperResponse>>> GetShippingOrderTodayByDeliveryStaff(DateTime time);
    public Task<OperationResult<bool>> ConfirmShippingOrderByShipper(Guid dailyOrderId);
    public Task<OperationResult<List<UserResponse>>> GetSDeliveryStaffByDailyOrder(Guid dailyOrderId);

}
