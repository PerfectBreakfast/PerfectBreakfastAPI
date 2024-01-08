namespace PerfectBreakfast.Application.Models.DaliyOrder.Response
{
    public record DailyOrderResponse
    {
        public Guid Id { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? OrderQuantity { get; set; }
        public DateOnly BookingDate { get; set; }
        public string Status { get; set; }

        public Guid? CompanyId { get; set; }
        public Guid? AdminId { get; set; }
    }
}
