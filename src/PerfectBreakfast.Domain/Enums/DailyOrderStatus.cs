using System.ComponentModel;

namespace PerfectBreakfast.Domain.Enums;

public enum DailyOrderStatus
{
    [Description("Đang chờ xử lý")]
    Pending = 1,

    [Description("Đã xử lý")]
    Fulfilled = 2,

    [Description("Đã hủy")]
    Cancelled = 3
}
