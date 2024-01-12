namespace PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

public class CreateSupplierCommissionRateRequest
{
    //public Guid? Id { get; set; }
    public int CommissionRate { get; set; }
    public Guid? FoodId { get; set; }
    public Guid? SupplierId { get; set; }
}