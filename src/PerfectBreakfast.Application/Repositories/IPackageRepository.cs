using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IPackageRepository : IGenericRepository<Package>
{
    Task<List<Package>> GetByPartner(Guid? partnerId);
}