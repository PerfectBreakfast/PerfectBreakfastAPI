using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Repositories;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected DbSet<TEntity> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _dbSet = context.Set<TEntity>();
    }
    public virtual Task<List<TEntity>> GetAllAsync() => _dbSet.ToListAsync();

    public virtual async Task<TEntity> GetByIdAsync(Guid id,params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var result = await _dbSet.FindAsync(id);
        // todo should throw exception when not found
        if (result == null)
            throw new NotFoundIdException($"Not Found by ID: [{id}] of [{typeof(TEntity).Name}]");
        return result;
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual void SoftRemove(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
        //_dbSet.Attach(entity).State = EntityState.Modified;
    }

    public virtual async Task AddRangeAsync(List<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void SoftRemoveRange(List<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public TEntity Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
        return entity;
    }

    public void RemoveRange(List<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual void UpdateRange(List<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }
    
    public IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        IQueryable<TEntity> items = _dbSet.AsNoTracking();
        if(includeProperties != null)
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        return items;
    }

    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        IQueryable<TEntity> items = _dbSet.AsNoTracking();
        if(includeProperties != null)
            foreach (var includeProperty in includeProperties)
            {
                items = items.Include(includeProperty);
            }
        return items.Where(predicate);
    }

    public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate, params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        return await FindAll(includeProperties).SingleOrDefaultAsync(predicate);
    }
}