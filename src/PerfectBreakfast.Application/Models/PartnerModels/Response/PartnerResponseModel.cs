

namespace PerfectBreakfast.Application.Models.PartnerModels.Response;

public record PartnerResponseModel(Guid Id,
    string Name,
    string Address,  
    string PhoneNumber ,
    decimal CommissionRate,
    double? Longitude,
    double? Latitude,
    List<string> Owners,
    List<string?>? AssignedCompanies,
    List<string?>? AssignedSuppliers,
    int MemberCount);