using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.FoodModels.Response;

public class TotalFoodForCompanyResponse
{
    public string? CompanyName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly? BookingDate { get; set; }
    public string? Status { get; set; }
    public List<TotalFoodResponse>? TotalFoodResponses { get; set; }
}