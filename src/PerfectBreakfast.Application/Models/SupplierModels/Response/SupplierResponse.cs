namespace PerfectBreakfast.Application.Models.SupplierModels.Response;

public record SupplierResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public string Address { get; set; } 
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
}