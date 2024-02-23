using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.FoodModels.Response;

public class TotalFoodForCompanyResponse
{
    public Guid? DailyOrderId { get; set; }
    public string? CompanyName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateOnly? BookingDate { get; set; }
    public string? Status { get; set; }
    public List<TotalFoodResponse>? TotalFoodResponses { get; set; }
}