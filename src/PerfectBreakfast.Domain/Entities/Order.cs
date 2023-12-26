using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class Order : BaseEntity
{
    public string Note { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public OrderStatus OrderStatus { get; set; }
    
    
    // relationship
    public Guid? WorkerId { get; set; }
    public Guid? ShipperId { get; set; }
    public Guid? DeliveryUnitId { get; set; }
    public Guid? ManagementUnitId { get; set; }
    public Guid? SupplierId { get; set; }
   
    public PaymentMethod? PaymentMethod { get; set; } // 1-1
    public User? Worker { get; set; }
    public User? Shipper { get; set; }
    public DeliveryUnit? DeliveryUnit { get; set; }
    public ManagementUnit? ManagementUnit { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<OrderDetail?> OrderDetails { get; set; }
    public ICollection<OrderHistory?> OrderHistories { get; set; }
}