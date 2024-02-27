using Microsoft.AspNetCore.Identity;

namespace PerfectBreakfast.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public int Code { get; set; } 
    public DateTime CreationDate { get; set; }
    public string? Image { get; set; }
    public string Name { get; set; }
    
    // relationship 
    public Guid? CompanyId { get; set; }
    public Guid? DeliveryId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? SupplierId { get; set; }
    
    
    public Company? Company { get; set; }
    public Delivery? Delivery { get; set; }
    public Partner? Partner { get; set; }
    public Supplier? Supplier { get; set; }
    
    public ICollection<Order?> OrdersWorker { get; set; }
    public ICollection<Order?> OrdersDeliveryStaff { get; set; }
    public ICollection<ShippingOrder?> ShippingOrders { get; set; }
    public ICollection<PartnerPayment?> PartnerPayments { get; set; }
    
    
}