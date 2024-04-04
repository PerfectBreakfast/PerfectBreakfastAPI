namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentDownloadForSupplier
    (
        string? PartnerName,
        string? CompanyName,
        List<FoodAssignmentResponse> FoodAssignmentResponses
        );