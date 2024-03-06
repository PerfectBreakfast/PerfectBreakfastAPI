using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Models.CategoryModels.Response;

public class CategoryDetailFood
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public List<FoodResponse?> FoodResponse { get; set; }
}