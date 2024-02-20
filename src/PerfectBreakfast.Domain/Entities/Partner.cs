namespace PerfectBreakfast.Domain.Entities;

public class Partner : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; }
    public int CommissionRate { get; set; }
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
    
    public ICollection<User?> Users { get; set; }
    public ICollection<SupplyAssignment?> SupplyAssignments { get; set; }
    public ICollection<PartnerPayment?> PartnerPayments { get; set; }
    public ICollection<SupplierFoodAssignment?> SupplierFoodAssignments { get; set; }
    public ICollection<Company?> Companies { get; set; }
    
}