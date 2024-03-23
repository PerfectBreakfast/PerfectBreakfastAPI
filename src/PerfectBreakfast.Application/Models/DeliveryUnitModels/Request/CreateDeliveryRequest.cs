namespace PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;

public record CreateDeliveryRequest(
    string Name,
    string Address,
    string PhoneNumber,
    int CommissionRate,
    double? Longitude,
    double? Latitude
    );