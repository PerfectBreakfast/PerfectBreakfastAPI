using PerfectBreakfast.Domain.Enums;
using Action = PerfectBreakfast.Domain.Enums.Action;

namespace PerfectBreakfast.Domain.Entities;

public class OrderHistory : BaseEntity
{
    public Action Action { get; set; }
    public OrderStatus OrderStatus { get; set; }
    
    public Guid? UserId { get; set; }
    public Guid? OrderId { get; set; }
    
    public User? User { get; set; }
    public Order? Order { get; set; }
}