namespace PerfectBreakfast.Application.Models.DaliyOrder.Request
{
    public record UpdateDailyOrderRequest
    {
        public Guid Id { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? OrderQuantity { get; set; }
        public DateOnly BookingDate { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? AdminId { get; set; }
    }
}
