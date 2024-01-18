using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.DaliyOrder.Response
{
    public record DailyOrderResponseExcel
    {
        public decimal? TotalPrice { get; set; }
        public int? OrderQuantity { get; set; }
        public DateOnly BookingDate { get; set; }
        public Company? Company { get; set; }
        public Dictionary<string, int> FoodCount { get; set; }
    }
}
