using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class PartnerPayment : BaseEntity
{
    public DateTime? RemittanceTime { get; set; }
    public decimal TotalPrice { get; set; }
    public PartnerPaymentStatus Status { get; set; }
    
    public Guid? DailyOrderId { get; set; }
    public Guid? AdminId { get; set; }
    public Guid? DeliveryUnitId { get; set; }
    public Guid? ManagementUnitId { get; set; }
    public Guid? SupplierId { get; set; }
    
    public DailyOrder? DailyOrder { get; set; }
    public User? User { get; set; }
    public DeliveryUnit? DeliveryUnit { get; set; }
    public ManagementUnit? ManagementUnit { get; set; }
    public Supplier? Supplier { get; set; }
}