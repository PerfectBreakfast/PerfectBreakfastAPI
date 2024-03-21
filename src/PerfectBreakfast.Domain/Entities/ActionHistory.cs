using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class ActionHistory : BaseEntity
{
    public ActionStatus Action { get; set; }
    public DailyOrderStatus Status { get; set; }
    
    public Guid? UserId { get; set; }
    public Guid? DailyOrderId { get; set; }
    
    public User? User { get; set; }
    public DailyOrder? DailyOrder { get; set; }
}