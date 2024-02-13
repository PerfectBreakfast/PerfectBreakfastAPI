namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request
{
    public record SupplierFoodsAssignmentRequest
    {
        public Guid? SupplierId { get; set; }
        public List<FoodAssignmentRequest> foodAssignmentRequests { get; set; }
    }
}
