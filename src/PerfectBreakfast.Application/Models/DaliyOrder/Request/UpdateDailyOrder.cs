using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.DaliyOrder.Request
{
    public record UpdateDailyOrder
    {
        public decimal? TotalPrice { get; set; }
        public int? OrderQuantity { get; set; }
        public DailyOrderStatus Status { get; set; }
    }
}
