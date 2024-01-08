namespace PerfectBreakfast.Domain.Enums;

public enum DailyOrderStatus
{
    Pending = 1,      // Đang chờ xử lý
    Fulfilled = 2,    // Đã xử lý
    Cancelled = 3    // Đã hủy
}