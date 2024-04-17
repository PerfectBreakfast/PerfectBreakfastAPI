namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

public record FoodAssignmentForPartner
{
    public Guid PackageId { get; set; }
    public List<Guid?> foodAssignmentIds { get; set; }
}