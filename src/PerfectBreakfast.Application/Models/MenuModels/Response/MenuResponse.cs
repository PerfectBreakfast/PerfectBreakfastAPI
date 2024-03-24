using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Models.MenuModels.Response
{
    public record MenuResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<ComboAndFoodResponse?>? ComboFoodResponses { get; set; }
        public List<FoodResponse?>? FoodResponses { get; set; }
    }
}
