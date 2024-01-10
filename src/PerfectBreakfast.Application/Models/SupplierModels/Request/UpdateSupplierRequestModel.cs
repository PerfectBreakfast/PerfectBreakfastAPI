namespace PerfectBreakfast.Application.Models.SupplierModels.Request;

public record UpdateSupplierRequestModel
{
    public string Name { get; set; } 
    public string Address { get; set; } 
    public double? Longitude { get; set; } 
}