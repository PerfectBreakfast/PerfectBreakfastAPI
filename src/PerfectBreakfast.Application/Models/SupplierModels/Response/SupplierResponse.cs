namespace PerfectBreakfast.Application.Models.SupplierModels.Response;

public record SupplierResponse(Guid Id,string Name,string Address,double? Longitude,double? Latitude,int MemberCount);