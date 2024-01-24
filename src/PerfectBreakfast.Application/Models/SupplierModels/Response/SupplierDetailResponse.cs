using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;

namespace PerfectBreakfast.Application.Models.SupplierModels.Response;

public record SupplierDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public string Address { get; set; } 
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; }
    public List<ManagementUnitDTO?> ManagementUnitDtos { get; set; }
}