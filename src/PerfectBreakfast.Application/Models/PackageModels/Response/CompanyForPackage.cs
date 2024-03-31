namespace PerfectBreakfast.Application.Models.PackageModels.Response;

public record CompanyForPackage
(
    string? CompanyName,
    List<MealForCompany> MealForCompanies
    );