namespace PerfectBreakfast.Domain.Entities;

public class ManagementUnit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int CommissionRate { get; set; }
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
    
    public ICollection<User?> Users { get; set; }
    public ICollection<Order?> Orders { get; set; }
    public ICollection<SupplyAssignment?> SupplyAssignments { get; set; }
    public ICollection<PartnerPayment?> PartnerPayments { get; set; }
    public ICollection<Company?> Companies { get; set; }
    
}