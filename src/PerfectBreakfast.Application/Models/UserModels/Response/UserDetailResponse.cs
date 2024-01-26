namespace PerfectBreakfast.Application.Models.UserModels.Response;

public record UserDetailResponse(Guid Id,string Email,string Code,string PhoneNumber,bool EmailConfirmed,bool LockoutEnabled,IList<string> Roles);