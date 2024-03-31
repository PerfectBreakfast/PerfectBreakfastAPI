using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.PackageModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IPackageService
{
    public Task<OperationResult<List<PackageResponse>>> GetPackageForPartner();
}