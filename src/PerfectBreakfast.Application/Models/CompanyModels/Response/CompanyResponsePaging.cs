namespace PerfectBreakfast.Application.Models.CompanyModels.Response
{
    public record CompanyResponsePaging
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public int MemberCount { get; set; }
        public string? ManagementUnit { get; set; } 
        public string? DeliveryUnit { get; set; }
    }
}
