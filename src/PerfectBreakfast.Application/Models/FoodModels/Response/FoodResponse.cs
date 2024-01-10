namespace PerfectBreakfast.Application.Models.FoodModels.Response
{
    public record FoodResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;
    }
}
