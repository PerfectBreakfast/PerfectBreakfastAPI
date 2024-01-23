namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request
{
    public record SupplierFoodAssignmentRequest
    {
        public Guid? SupplierId { get; set; }
        public List<FoodAssignmentRequest> foodAssignmentRequests { get; set; }
    }
}
