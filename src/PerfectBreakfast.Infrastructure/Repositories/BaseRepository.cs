using Microsoft.EntityFrameworkCore;
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

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        var result = await _dbSet.FindAsync(id);
        // todo should throw exception when not found
        return result;
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void SoftRemove(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual async Task AddRangeAsync(List<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void SoftRemoveRange(List<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }
        
    public virtual void UpdateRange(List<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }
}