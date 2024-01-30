namespace PerfectBreakfast.Application.Models.PartnerModels.Request;

public record UpdateManagementUnitRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}