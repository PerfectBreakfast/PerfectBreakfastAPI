namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request
{
    public record FoodAssignmentRequest
    {
        public int AmountCooked { get; set; }
        public Guid? FoodId { get; set; }
    }
}
