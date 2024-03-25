using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.PartnerModels.Response;

public record PartnerDetailResponse(
    Guid? Id,
    string Name,
    string Address,
    string PhoneNumber,
    int CommissionRate,
    double? Longitude,
    double? Latitude,
    List<SupplierDTO?> SupplierDTO,
    List<CompanyResponsePaging> Companies
);