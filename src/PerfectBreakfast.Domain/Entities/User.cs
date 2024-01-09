using Microsoft.AspNetCore.Identity;

namespace PerfectBreakfast.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public int Code { get; set; } 
    public DateTime CreationDate { get; set; }
    
    // relationship 
    public Guid? CompanyId { get; set; }
    public Guid? DeliveryUnitId { get; set; }
    public Guid? ManagementUnitId { get; set; }
    public Guid? SupplierId { get; set; }
    
    
    public Company? Company { get; set; }
    public DeliveryUnit? DeliveryUnit { get; set; }
    public ManagementUnit? ManagementUnit { get; set; }
    public Supplier? Supplier { get; set; }
    
    public ICollection<OrderHistory?> OrderHistories { get; set; }
    public ICollection<Order?> OrdersWorker { get; set; }
    public ICollection<DailyOrder?> DailyOrders { get; set; }
    public ICollection<ShippingOrder?> ShippingOrders { get; set; }
    public ICollection<PartnerPayment?> PartnerPayments { get; set; }
    
    
}