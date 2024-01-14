using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Response;

namespace PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;

public record SupplierMoreFoodRespone
{
    public Guid? SupplierResponseId { get; set; }
    public int CommissionRate { get; set; }
    public List<FoodResponse> FoodResponses { get; set; } = new List<FoodResponse>();
    
}