namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

public record DeliveryUnitResponseModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
}