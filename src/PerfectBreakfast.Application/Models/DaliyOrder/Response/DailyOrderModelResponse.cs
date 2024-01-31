using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.DaliyOrder.Response;

public record DailyOrderModelResponse(
    Guid Id,
    decimal? TotalPrice,
    int? OrderQuantity ,
    string Status,
    CompanyDto? Company);