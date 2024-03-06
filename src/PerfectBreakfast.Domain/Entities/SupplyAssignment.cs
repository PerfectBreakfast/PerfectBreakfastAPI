namespace PerfectBreakfast.Domain.Entities;

public class SupplyAssignment 
{
    public Guid? SupplierId { get; set; }
    public Guid? PartnerId { get; set; }
    
    public Supplier? Supplier { get; set; }
    public Partner? Partner { get; set; }
    
    
}