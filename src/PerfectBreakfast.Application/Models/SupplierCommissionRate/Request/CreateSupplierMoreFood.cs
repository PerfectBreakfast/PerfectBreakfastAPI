using System.Collections;

namespace PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

public record CreateSupplierMoreFood() 
{
    public List<CreateSupplierCommissionRateRequest?> FoodId { get; set; }
   
}