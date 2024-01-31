namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

public record DeliveryResponseModel(
    Guid Id,
    string Name,
    string Address,
    //int CommissionRate,
    double? Longitude,
    double? Latitude,
    List<string> Owners,
    List<string> AssignedCompanies,
    int MemberCount);
