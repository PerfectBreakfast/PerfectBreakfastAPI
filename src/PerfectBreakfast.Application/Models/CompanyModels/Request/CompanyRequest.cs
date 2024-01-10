﻿namespace PerfectBreakfast.Application.Models.CompanyModels.Request
{
    public record CompanyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid? ManagementUnitId { get; set; }
        public Guid? DeliveryUnitId { get; set; }
    }
}
