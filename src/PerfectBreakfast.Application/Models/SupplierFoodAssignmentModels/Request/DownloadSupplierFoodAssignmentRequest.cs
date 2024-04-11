namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

public record DownloadSupplierFoodAssignmentRequest
    (
        DateOnly BookingDate, 
        List<Guid> PackageIds
        );