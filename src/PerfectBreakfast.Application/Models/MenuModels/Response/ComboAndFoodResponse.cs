using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Models.MenuModels.Response
{
    public record ComboAndFoodResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Content { get; set; }
        public List<FoodResponse?> FoodResponses { get; set; }
    }
}
