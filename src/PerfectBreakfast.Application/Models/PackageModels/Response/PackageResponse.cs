namespace PerfectBreakfast.Application.Models.PackageModels.Response;

public record PackageResponse
(
    DateOnly? BookingDate,
    List<CompanyForPackage?> CompanyForPackages
);