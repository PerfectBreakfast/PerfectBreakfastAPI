using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;

public record ManagementUnitResponseModel(Guid Id,string Name,string Address,int CommissionRate,double? Longitude,double? Latitude,List<string> Owners,List<string?> SupplierName,int MemberCount);