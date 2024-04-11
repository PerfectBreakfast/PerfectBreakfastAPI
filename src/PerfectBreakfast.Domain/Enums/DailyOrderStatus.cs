namespace PerfectBreakfast.Domain.Enums;

public enum DailyOrderStatus
{
    Initial,  
    Processing,
    Divided,
    Cooking,
    Waiting,
    Delivering,    
    Complete,
    NoOrders,// trạng thái empty , là k có order nào đặt trong cái dailyOrder này 
    Cancel // trạng thái xảy ra khi daily order quá ngày giao mà không đổi trạng thái
}
