namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;

public record DeliveryUnitResponseModel(
    Guid Id,
    string Name,
    string Address,
    double? Longitude,
    double? Latitude,
    int MemberCount);
