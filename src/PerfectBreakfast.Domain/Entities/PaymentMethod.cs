namespace PerfectBreakfast.Domain.Entities;

public class PaymentMethod : BaseEntity
{
    public required string Name { get; set; }
    
    // relationship
    public ICollection<Order?> Orders { get; set; }
}