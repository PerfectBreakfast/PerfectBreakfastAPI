namespace PerfectBreakfast.Application.Models.FoodModels.Response;

    public record TotalFoodResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public string? FoodName { get; set; }
    }
