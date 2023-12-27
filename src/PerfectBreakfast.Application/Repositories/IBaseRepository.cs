using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(Guid id,params Expression<Func<TEntity, object>>[] includeProperties);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void UpdateRange(List<TEntity> entities);
    void SoftRemove(TEntity entity);
    Task AddRangeAsync(List<TEntity> entities);
    void SoftRemoveRange(List<TEntity> entities);
    public IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[]? includeProperties);
    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[]? includeProperties);
    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate, params Expression<Func<TEntity, object>>[]? includeProperties);
}