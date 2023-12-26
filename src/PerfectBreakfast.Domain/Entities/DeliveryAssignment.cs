namespace PerfectBreakfast.Domain.Entities;

public class DeliveryAssignment : BaseEntity
{
    public Guid? DeliveryUnitId { get; set; }
    public Guid? ManagementUnitId { get; set; }
    
    public DeliveryUnit? DeliveryUnit { get; set; }
    public ManagementUnit? ManagementUnit { get; set; }
}