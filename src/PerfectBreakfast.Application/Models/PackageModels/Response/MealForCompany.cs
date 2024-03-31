namespace PerfectBreakfast.Application.Models.PackageModels.Response;

public record MealForCompany
(
    string? MealType,
    List<PackageForMeal> PackageForMeals
    );