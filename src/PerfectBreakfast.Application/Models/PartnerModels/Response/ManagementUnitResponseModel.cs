

namespace PerfectBreakfast.Application.Models.PartnerModels.Response;

public record ManagementUnitResponseModel(Guid Id,string Name,string Address,int CommissionRate,double? Longitude,double? Latitude,List<string> Owners,List<string> AssignedCompanies,List<string?> AssignedSuppliers,int MemberCount);