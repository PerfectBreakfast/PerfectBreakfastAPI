namespace PerfectBreakfast.Domain.Entities;

public class SupplierCommissionRate : BaseEntity
{
    public decimal CommissionRate { get; set; }
    
    // relationship 
    public Guid? FoodId { get; set; }
    public Guid? SupplierId { get; set; }
    
    public Food? Food { get; set; }
    public Supplier? Supplier { get; set; }
    
    public ICollection<SupplierFoodAssignment?> SupplierFoodAssignments { get; set; }

    
}