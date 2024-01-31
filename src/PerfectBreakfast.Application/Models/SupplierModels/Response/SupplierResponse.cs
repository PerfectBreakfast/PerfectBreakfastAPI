namespace PerfectBreakfast.Application.Models.SupplierModels.Response;

public record SupplierResponse(Guid Id,string Name,string Address, string PhoneNumber, double? Longitude,double? Latitude,List<string> Owners,List<string?> ManagementUnitName,int MemberCount);