using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.MealModels.Response;
using PerfectBreakfast.Application.Models.PartnerModels.Response;

namespace PerfectBreakfast.Application.Models.CompanyModels.Response;

public record CompanyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public int MemberCount { get; set; }
    public List<MealResponse?> Meals { get; set; }
    public PartnerResponseModel? Partner { get; set; }
    public DeliveryResponseModel? Delivery { get; set; }
}