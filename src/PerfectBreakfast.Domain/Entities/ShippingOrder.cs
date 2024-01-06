using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class ShippingOrder : BaseEntity
{
    public ShippingStatus Status { get; set; }
    
    
    // relationship
    public Guid? DailyOrderId { get; set; }
    public Guid? ShipperId { get; set; }
    
    public DailyOrder? DailyOrder { get; set; }
    public User? Shipper { get; set; }
}