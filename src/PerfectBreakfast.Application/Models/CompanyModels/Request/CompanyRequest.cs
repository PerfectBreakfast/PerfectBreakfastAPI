﻿namespace PerfectBreakfast.Application.Models.CompanyModels.Request
{
    public record CompanyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public TimeOnly? StartWorkHour { get; set; }
        public Guid? PartnerId { get; set; }
        public Guid? DeliveryId { get; set; }
    }
}
