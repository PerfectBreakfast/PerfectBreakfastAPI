namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

public record DownloadFoodAssignmentForPartnerRequest
    (
        DateOnly BookingDate, 
        List<Guid> PackageIds,
        Guid SupplierId
        );