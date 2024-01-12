namespace PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

public class UpdateSupplierCommissionRateRequest
{
    public Guid? Id { get; set; }
    public int CommissionRate { get; set; }
    public Guid? FoodId { get; set; }
    public Guid? SupplierId { get; set; }
}