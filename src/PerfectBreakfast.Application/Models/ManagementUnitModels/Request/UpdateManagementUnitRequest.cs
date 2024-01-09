namespace PerfectBreakfast.Application.Models.ManagementUnitModels.Request;

public record UpdateManagementUnitRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}