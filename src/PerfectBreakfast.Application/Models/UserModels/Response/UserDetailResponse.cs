namespace PerfectBreakfast.Application.Models.UserModels.Response;

public record UserDetailResponse(Guid Id,string Name,string Email,string Code,string PhoneNumber,bool EmailConfirmed,bool LockoutEnabled,string CompanyName,IList<string> Roles);