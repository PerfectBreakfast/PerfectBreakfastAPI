namespace PerfectBreakfast.Application.Models.UserModels.Response;

public record UserDetailResponse(Guid Id,string Name,string Email,string Image,string Code,string PhoneNumber,bool EmailConfirmed,bool LockoutEnabled, Guid CompanyId,string? CompanyName,IList<string> Roles);