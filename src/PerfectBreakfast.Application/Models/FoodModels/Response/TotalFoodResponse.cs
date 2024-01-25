namespace PerfectBreakfast.Application.Models.FoodModels.Response
{
    public record TotalFoodResponse
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
