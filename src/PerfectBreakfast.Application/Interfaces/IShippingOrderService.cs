using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;


namespace PerfectBreakfast.Application.Interfaces;

public interface IShippingOrderService
{
    public Task<OperationResult<ShippingOrderRespone>> CreateShippingOrder(CreateShippingOrderRequest requestModel);
}
