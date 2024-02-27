using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.PartnerModels.Response;

public record PartnerDetailResponse
{
    public Guid? Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; }
    public int CommissionRate { get; set; }
    public double? Longitude { get; set; } 
    public double? Latitude { get; set; } 
    public List<SupplierDTO?> SupplierDTO { get; set; }
    public List<CompanyResponsePaging> Companies { get; set; }
}