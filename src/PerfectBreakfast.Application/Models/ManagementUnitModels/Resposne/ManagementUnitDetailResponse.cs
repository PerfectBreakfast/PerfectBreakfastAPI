using PerfectBreakfast.Application.Models.SupplierModels.Response;

namespace PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;

public record ManagementUnitDetailResponse
{
    public Guid? Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int CommissionRate { get; set; }
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
    public List<SupplierDTO?> SupplierDTO { get; set; }
}