using System.ComponentModel;

namespace PerfectBreakfast.Domain.Enums;

public enum DailyOrderStatus
{
    Initial,
    Processing,
    Cooking,
    Waiting,
    Delivering,    
    Complete,
    NoOrders    // trạng thái empty , là k có order nào đặt trong cái dailyOrder này 
}
