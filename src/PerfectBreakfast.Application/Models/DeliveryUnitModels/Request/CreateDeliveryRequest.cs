namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;

public record CreateDeliveryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int CommissionRate { get; set; } 
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
}