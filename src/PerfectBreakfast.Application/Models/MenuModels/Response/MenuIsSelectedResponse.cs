using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Models.MenuModels.Response;

public record MenuIsSelectedResponse
(
    Guid Id,
    string Name,
    bool IsSelected,
    bool IsDeleted,
    DateTime MenuDate,
    List<ComboAndFoodResponse?> ComboFoodResponses,
    List<FoodResponse?> FoodResponses
);