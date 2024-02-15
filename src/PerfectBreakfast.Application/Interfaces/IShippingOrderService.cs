

using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface IShippingOrderService
{
    public Task<OperationResult<ShippingOrderRespone>> CreateShippingOrder(CreateShippingOrderRequest requestModel);
}
