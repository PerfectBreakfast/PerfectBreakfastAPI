namespace PerfectBreakfast.Application.Models.PaymentMethodModels.Response
{
    public record PaymentMethodResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
