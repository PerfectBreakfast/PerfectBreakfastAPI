namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request
{
    public record SupplierFoodAssignmentRequest
    {
        public decimal? ReceivedAmount { get; set; }
        public Guid? FoodId { get; set; }
    }
}
