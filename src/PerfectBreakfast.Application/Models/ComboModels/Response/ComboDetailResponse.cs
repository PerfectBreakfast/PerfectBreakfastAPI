using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Models.ComboModels.Response;

public record ComboDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public string? Image { get; set; }
    public string Foods { get; set; }
    public decimal comboPrice { get; set; }
    public List<FoodResponseCategory?> FoodResponses { get; set; }
}