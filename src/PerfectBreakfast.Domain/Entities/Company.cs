namespace PerfectBreakfast.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public TimeOnly? StartWorkHour { get; set; }
    
    // relationship
    public Guid? ManagementUnitId { get; set; }
    public Guid? DeliveryUnitId { get; set; }
    
    public ManagementUnit? ManagementUnit { get; set; }
    public DeliveryUnit? DeliveryUnit { get; set; }
    
    public ICollection<User?> Workers { get; set; }
    public ICollection<DailyOrder?> DailyOrders { get; set; }
}