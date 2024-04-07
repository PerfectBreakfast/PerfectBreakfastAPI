using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class Package : BaseEntity
{
    public int TotalFoodAssignQuantity { get; set; }
    public int MissingQuantity { get; set; }
    public int SuccessfulQuantity { get; set; }
    
    // relationship 
    
    public Guid? DailyOrderId { get; set; }
    public Guid? PartnerId { get; set; }

    public DailyOrder? DailyOrder { get; set; }
    public Partner? Partner { get; set; }
    
    public ICollection<SupplierFoodAssignment?> SupplierFoodAssignments { get; set; }
}