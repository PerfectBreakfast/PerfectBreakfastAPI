using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.DailyOrder.Response;

public record DailyOrderModelResponse(
    Guid Id,
    //string Meal,
    decimal? TotalPrice,
    int? OrderQuantity ,
    string Status);