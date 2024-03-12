namespace PerfectBreakfast.Application.Models.FoodModels.Response;

public record TotalFoodForPartnerResponse
{
    public Guid? DailyOrderId { get; set; }
    public string? Meal { get; set; }
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? Status { get; set; } 
    public List<TotalFoodResponse>? TotalFoodResponses { get; set; }
}