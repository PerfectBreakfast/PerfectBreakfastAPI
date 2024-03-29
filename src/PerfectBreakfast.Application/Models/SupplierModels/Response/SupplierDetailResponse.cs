using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;

namespace PerfectBreakfast.Application.Models.SupplierModels.Response;

public record SupplierDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public string Address { get; set; } 
    public string PhoneNumber { get; set; }
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; }
    public List<PartnerDTO?> ManagementUnitDtos { get; set; }
    public List<CommissionRateResponse?> CommissionRates { get; set; }
}