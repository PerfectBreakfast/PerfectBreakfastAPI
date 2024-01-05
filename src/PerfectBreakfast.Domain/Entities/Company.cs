namespace PerfectBreakfast.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public TimeOnly? StartWorkHour { get; set; }
    
    public ICollection<User?> Workers { get; set; }
}