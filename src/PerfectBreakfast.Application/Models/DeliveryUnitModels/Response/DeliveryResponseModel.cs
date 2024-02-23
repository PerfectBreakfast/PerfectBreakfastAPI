namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

public record DeliveryResponseModel(
    Guid Id,
    string Name,
    string Address,
    string PhoneNumber ,
    int CommissionRate,
    double? Longitude,
    double? Latitude,
    List<string> Owners,
    List<string> AssignedCompanies,
    int MemberCount);
