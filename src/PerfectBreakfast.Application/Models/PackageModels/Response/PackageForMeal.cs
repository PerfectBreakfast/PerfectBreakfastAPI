namespace PerfectBreakfast.Application.Models.PackageModels.Response;

public record PackageForMeal
    (
        int TotalFoodAssignQuantity,
        int MissingQuantity,
        int SuccessfulQuantity,
        List<FoodAssignmentForPackage> FoodAssignmentForPackages
        );