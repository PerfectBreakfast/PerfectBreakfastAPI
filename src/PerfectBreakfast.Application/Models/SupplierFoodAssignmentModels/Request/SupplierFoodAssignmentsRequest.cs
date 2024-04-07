namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

public record SupplierFoodAssignmentsRequest
{
    public Guid DailyOrderId { get; set; }
    public List<SupplierFoodAssignmentRequest>? SupplierFoodAssignmentRequest { get; set; }
}