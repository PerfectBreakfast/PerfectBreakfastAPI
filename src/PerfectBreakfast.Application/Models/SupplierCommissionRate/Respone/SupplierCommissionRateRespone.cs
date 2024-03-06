using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Response;

namespace PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;

public record SupplierCommissionRateRespone
{
    public Guid? Id { get; set; }
    public int CommissionRate { get; set; }
    public FoodResponse? FoodResponses { get; set; }
    public SupplierResponse? SupplierResponse{get; set; }
}