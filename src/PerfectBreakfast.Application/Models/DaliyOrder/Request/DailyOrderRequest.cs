namespace PerfectBreakfast.Application.Models.DaliyOrder.Request
{
    public record DailyOrderRequest
    {
        public DateOnly BookingDate { get; set; }

        public Guid? CompanyId { get; set; }
        public Guid? AdminId { get; set; }
    }
}
