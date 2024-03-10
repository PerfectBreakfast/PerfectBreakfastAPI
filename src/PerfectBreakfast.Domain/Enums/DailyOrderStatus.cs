using System.ComponentModel;

namespace PerfectBreakfast.Domain.Enums;

public enum DailyOrderStatus
{
    Initial,
    Processing,
    Cooking,
    Waiting,
    Delivering,    
    Complete 
}
