namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;

public record UpdateDeliveryUnitRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}