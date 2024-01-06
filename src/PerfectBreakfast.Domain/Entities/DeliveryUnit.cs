namespace PerfectBreakfast.Domain.Entities;

public class DeliveryUnit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
    
    public ICollection<User?> Users { get; set; }
    public ICollection<PartnerPayment?> PartnerPayments { get; set; }
    public ICollection<Company?> Companies { get; set; }
}