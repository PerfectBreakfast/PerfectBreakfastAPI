using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Models.ComboModels.Response
{
    public record ComboResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public List<FoodResponse?> FoodResponses { get; set; }
    }
}
