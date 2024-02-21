﻿namespace PerfectBreakfast.Application.Models.PartnerModels.Request;

public record UpdatePartnerRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int CommissionRate { get; set; }
}