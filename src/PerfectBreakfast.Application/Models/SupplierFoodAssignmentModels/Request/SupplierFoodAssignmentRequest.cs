namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

public record SupplierFoodAssignmentRequest
{
    public Guid SupplierId { get; set; }
    public Guid FoodId { get; set; }
    public int AmountCooked { get; set; }
}