namespace PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

public record CreateSupplierCommissionRateRequest
{
    //public Guid FoodId { get; }
    public List<Guid> FoodIds { get; set; } // List of FoodIds
    public Guid SupplierId { get; set; }    // Single SupplierId
    public int CommissionRate { get; set; }
}