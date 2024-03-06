using PerfectBreakfast.Application.Models.FoodModels.Response;


namespace PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;

public record SupplierMoreFoodRespone
{
    
    public List<FoodResponse> FoodResponses { get; set; } = new List<FoodResponse>();
    
}