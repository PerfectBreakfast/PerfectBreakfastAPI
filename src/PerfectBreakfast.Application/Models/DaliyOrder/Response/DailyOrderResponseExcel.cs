using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.DaliyOrder.Response
{
    public record DailyOrderResponseExcel
    {
        public decimal? TotalPrice { get; set; }
        public int? OrderQuantity { get; set; }
        public DateOnly BookingDate { get; set; }
        public List<Food> FoodResponses { get; set; }
        public Company? Company { get; set; }
    }
}
