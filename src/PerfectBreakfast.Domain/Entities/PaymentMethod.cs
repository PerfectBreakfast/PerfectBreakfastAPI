namespace PerfectBreakfast.Domain.Entities;

public class PaymentMethod : BaseEntity
{
    public required string Name { get; set; }
    
    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }
}