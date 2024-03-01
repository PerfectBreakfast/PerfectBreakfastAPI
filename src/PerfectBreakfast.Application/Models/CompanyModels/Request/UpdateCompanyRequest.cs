namespace PerfectBreakfast.Application.Models.CompanyModels.Request
{
    public record UpdateCompanyRequest
    {
        public string? Name { get; set; } 
        public string? PhoneNumber { get; set; } 
        public string? Email { get; set; } 
        public string? Address { get; set; } 
        public Guid? PartnerId { get; set; }
        public Guid? DeliveryId { get; set; }
    }
}
