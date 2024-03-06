using System.Linq.Expressions;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories;

public interface IGenericRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<Pagination<TEntity>> ToPagination(int pageNumber = 0, int pageSize = 10, Expression<Func<TEntity, bool>>? predicate = null,params IncludeInfo<TEntity>[] includeProperties);
}