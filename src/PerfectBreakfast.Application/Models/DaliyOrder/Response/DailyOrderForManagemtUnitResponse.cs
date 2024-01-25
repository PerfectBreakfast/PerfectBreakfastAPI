namespace PerfectBreakfast.Application.Models.DaliyOrder.Response
{
    public record DailyOrderForManagemtUnitResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public TimeOnly? StartWorkHour { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? OrderQuantity { get; set; }
        public DateOnly BookingDate { get; set; }

    }
}
