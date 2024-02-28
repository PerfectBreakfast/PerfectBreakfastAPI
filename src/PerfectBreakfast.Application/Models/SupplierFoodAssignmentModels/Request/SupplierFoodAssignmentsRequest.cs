namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

public class SupplierFoodAssignmentsRequest
{
    public Guid? DailyOrderId { get; set; }
    public string? Meal { get; set; }
    public List<SupplierFoodAssignmentRequest>? SupplierFoodAssignmentRequest { get; set; }
}