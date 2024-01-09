namespace PerfectBreakfast.Domain.Entities;

public class SupplyAssignment 
{
    public Guid? SupplierId { get; set; }
    public Guid? ManagementUnitId { get; set; }
    
    public Supplier? Supplier { get; set; }
    public ManagementUnit? ManagementUnit { get; set; }
    
    
}