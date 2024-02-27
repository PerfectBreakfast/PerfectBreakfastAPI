namespace PerfectBreakfast.Domain.Entities;

public class MealSubscription : BaseEntity
{
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    
    // Relationship
    public Guid? MealId { get; set; }
    public Guid? CompanyId { get; set; }
    
    public Meal? Meal { get; set; }
    public Company? Company { get; set; }

    public ICollection<DailyOrder?> DailyOrders { get; set; }
}