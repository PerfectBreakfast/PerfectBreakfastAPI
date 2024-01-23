namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record FoodAssignmentResponse
    {
        public int AmountCooked { get; set; }
        public Guid? FoodId { get; set; }
    }
}
