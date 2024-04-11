namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentDownloadForSupplier
    (
        Guid? Id,
        string? PartnerName,
        string? CompanyName,
        List<FoodAssignmentResponse> FoodAssignmentResponses
        );