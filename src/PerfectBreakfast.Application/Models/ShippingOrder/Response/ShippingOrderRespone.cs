

namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

record class ShippingOrderRespone
{
    public Guid? DailyOrderId { get; set; }
    public Guid? ShipperId { get; set; }
}
