using PerfectBreakfast.Application.Models.CategoryModels.Response;

namespace PerfectBreakfast.Application.Models.FoodModels.Response;

public record FoodResponseCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public int FoodStatus { get; set; }
    public CategoryResponse? CategoryResponse { get; set; }
}