namespace PerfectBreakfast.Application.Models.PartnerModels.Request;

public record UpdatePartnerRequest
{
    public string? Name { get; set; } 
    public string? Address { get; set; } 
    public string? PhoneNumber { get; set; } 
    public int? CommissionRate { get; set; }
}