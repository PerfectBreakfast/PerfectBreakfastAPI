namespace PerfectBreakfast.Domain.Entities;

public class DailyOrder : BaseEntity
{
    public decimal TotalPrice { get; set; }
    public int OrderQuantity { get; set; }
    public DateOnly BookingDate { get; set; }
    public string Status {get; set; }
    
    // relationship
    public Guid? CompanyId { get; set; }
    public Guid? AdminId { get; set; }
    
    public Company? Company { get; set; }
    public User? User { get; set; }
    
    public ICollection<OrderHistory?> OrderHistories { get; set; }
    public ICollection<User?> Users { get; set; }

}