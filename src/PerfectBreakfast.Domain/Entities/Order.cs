using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class Order : BaseEntity
{
    public string? Note { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public long OrderCode { get; set; }
    
    
    // relationship
    public Guid? WorkerId { get; set; }
    public Guid? DeliveryStaffId { get; set; }
    public Guid? DailyOrderId { get; set; }
    public Guid? PaymentMethodId { get; set; }
   
    public PaymentMethod? PaymentMethod { get; set; } 
    public User? Worker { get; set; }
    public User? DeliveryStaff { get; set; }
    public DailyOrder? DailyOrder { get; set; }

    public ICollection<OrderDetail?> OrderDetails { get; set; }
}