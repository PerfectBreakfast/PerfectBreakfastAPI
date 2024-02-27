namespace PerfectBreakfast.Domain.Entities;

public class Company : BaseEntity
{
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; } 
    public required string Email { get; set; } 
    public required string Address { get; set; } 
    
    // relationship
    public Guid? PartnerId { get; set; }
    public Guid? DeliveryId { get; set; }
    
    public Partner? Partner { get; set; }
    public Delivery? Delivery { get; set; }
    
    public ICollection<User?> Workers { get; set; }
    public ICollection<DailyOrder?> DailyOrders { get; set; }
    public ICollection<MealSubscription?> MealSubscriptions { get; set; }
}