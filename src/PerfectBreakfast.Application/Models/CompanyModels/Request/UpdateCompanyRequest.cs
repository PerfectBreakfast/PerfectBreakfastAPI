namespace PerfectBreakfast.Application.Models.CompanyModels.Request
{
    public record UpdateCompanyRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public TimeOnly? StartWorkHour { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? ManagementUnitId { get; set; }
        public Guid? DeliveryUnitId { get; set; }
    }
}
