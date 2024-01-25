using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<Company?> GetCompanyById(Guid id);
}