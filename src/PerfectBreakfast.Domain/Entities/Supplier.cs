using System.Collections;

namespace PerfectBreakfast.Domain.Entities;

public class Supplier : BaseEntity
{
    public required string Name { get; set; } 
    public required string Address { get; set; } 
    public required string PhoneNumber { get; set; }
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
    
    public ICollection<User?> Users { get; set; }
    public ICollection<PartnerPayment?> PartnerPayments { get; set; }
    public ICollection<SupplyAssignment?> SupplyAssignments { get; set; }
    public ICollection<SupplierCommissionRate?> SupplierCommissionRates { get; set; }
}