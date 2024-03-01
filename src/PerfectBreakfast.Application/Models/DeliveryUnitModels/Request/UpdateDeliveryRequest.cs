namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;

public record UpdateDeliveryRequest
{
    public string? Name { get; set; } 
    public string? Address { get; set; } 
    public int? CommissionRate { get; set; } 
}