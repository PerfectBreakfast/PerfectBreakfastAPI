namespace PerfectBreakfast.Application.Models.UserModels.Response;

public record UserDetailResponse(Guid Id,string Email,string Code,string PhoneNumber,string CompanyName,bool EmailConfirmed,bool LockoutEnabled);