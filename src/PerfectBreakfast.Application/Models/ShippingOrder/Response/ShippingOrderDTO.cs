using PerfectBreakfast.Application.Models.DailyOrder.Response;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

public record ShippingOrderDTO()
{
    public Guid? ShipperId { get; set; }
    // This should be a single DTO, not a list, based on your entity relationship
    public DailyOrderResponse? DailyOrderDTO { get; set; } 
}