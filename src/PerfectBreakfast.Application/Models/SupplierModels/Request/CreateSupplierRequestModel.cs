namespace PerfectBreakfast.Application.Models.SupplierModels.Request;

public record CreateSupplierRequestModel
{
    public string Name { get; set; } 
    public string Address { get; set; } 
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
}