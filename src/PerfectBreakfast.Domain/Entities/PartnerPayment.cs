using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class PartnerPayment : BaseEntity
{
    public DateTime? RemittanceTime { get; set; }
    public decimal TotalPrice { get; set; }
    public PartnerPaymentStatus Status { get; set; }
    
    public Guid? DailyOrderId { get; set; }
    public Guid? SupperAdminId { get; set; }
    public Guid? DeliveryId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? SupplierId { get; set; }
    
    public DailyOrder? DailyOrder { get; set; }
    public User? SupperAdmin { get; set; }
    public Delivery? Delivery { get; set; }
    public Partner? Partner { get; set; }
    public Supplier? Supplier { get; set; }
}