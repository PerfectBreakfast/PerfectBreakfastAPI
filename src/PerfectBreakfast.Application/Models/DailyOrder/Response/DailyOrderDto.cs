using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.PartnerModels.Response;

namespace PerfectBreakfast.Application.Models.DailyOrder.Response;

public record DailyOrderDto(
    Guid Id,
    DateOnly BookingDate,
    TimeOnly PickupTime,
    TimeOnly HandoverTime,
    string Status,
    string Meal,
    CompanyDto Company,
    PartnerDTO Partner
    );