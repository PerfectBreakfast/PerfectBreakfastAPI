using PerfectBreakfast.Application.Models.CompanyModels.Response;

namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

public record DeliveryDetailResponse(
    Guid Id,
    string Name,
    string Address,
    string PhoneNumber ,
    int CommissionRate,
    double? Longitude,
    double? Latitude,
    List<string> Owners,
    List<CompanyDto?> AssignedCompanies,
    int MemberCount);