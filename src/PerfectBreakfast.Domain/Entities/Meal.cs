namespace PerfectBreakfast.Domain.Entities;

public class Meal : BaseEntity
{
    public required string MealType { get; set; }
    
    // Relationship 
    public ICollection<MealSubscription?> MealSubscriptions { get; set; }
    public ICollection<Order?> Orders { get; set; }
}