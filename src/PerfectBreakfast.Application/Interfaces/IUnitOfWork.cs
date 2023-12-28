using Microsoft.EntityFrameworkCore.Storage;
using PerfectBreakfast.Application.Repositories;

namespace PerfectBreakfast.Application.Interfaces;

public interface IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public ICompanyRepository CompanyRepository { get; }
    public IRoleRepository RoleRepository { get; }
    public Task<int> SaveChangeAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
