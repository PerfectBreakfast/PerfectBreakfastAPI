using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Repositories;

public class GenericRepository<TEntity> : BaseRepository<TEntity>, IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected DbSet<TEntity> _dbSet;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;

        public GenericRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context)
        {
            _dbSet = context.Set<TEntity>();
            _timeService = timeService;
            _claimsService = claimsService;
        }

        public override async Task<TEntity> GetByIdAsync(Guid id,params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var result = await FindAll(includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
            // todo should throw exception when not found
            if (result == null)
                throw new NotFoundIdException($"Not Found by ID: [{id}] of [{typeof(TEntity).Name}]");
            return result;
        }

        public override async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.CreationDate = _timeService.GetCurrentTime();
            entity.CreatedBy = _claimsService.GetCurrentUserId;
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public override void SoftRemove(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.DeleteBy = _claimsService.GetCurrentUserId;
            _dbSet.Update(entity);
        }

        public override void Update(TEntity entity)
        {
            entity.ModificationDate = _timeService.GetCurrentTime();
            entity.ModificationBy = _claimsService.GetCurrentUserId;
            //_dbSet.Attach(entity).State = EntityState.Modified;
            _dbSet.Update(entity);
        }

        public override async Task AddRangeAsync(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreationDate = _timeService.GetCurrentTime();
                entity.CreatedBy = _claimsService.GetCurrentUserId;
            }
            await _dbSet.AddRangeAsync(entities);
        }

        public override void SoftRemoveRange(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.DeletionDate = _timeService.GetCurrentTime();
                entity.DeleteBy = _claimsService.GetCurrentUserId;
            }
            _dbSet.UpdateRange(entities);
        }
        
        public async Task<Pagination<TEntity>> ToPagination(int pageIndex = 0, int pageSize = 10, 
            Expression<Func<TEntity, bool>>? predicate = null,
            params IncludeInfo<TEntity>[] includeProperties)
        {
            IQueryable<TEntity> itemsQuery = null;
            if (predicate != null) 
            {
                itemsQuery = _dbSet.Where(predicate); 
            }
            itemsQuery = itemsQuery.OrderByDescending(x => x.CreationDate);
            var itemCount = await itemsQuery.CountAsync();
            // Xử lý các thuộc tính include và thenInclude
            foreach (var includeProperty in includeProperties)
            {
                var queryWithInclude = itemsQuery.Include(includeProperty.NavigationProperty);
                foreach (var thenInclude in includeProperty.ThenIncludes)
                {
                    queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
                }
                itemsQuery = queryWithInclude;
            }
            
            var items = await itemsQuery
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
            
            var result = new Pagination<TEntity>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = itemCount,
                Items = items,
            };
            return result;
        }

        public async Task<TEntity> GetByIdAndIncludeAsync(Guid id, params IncludeInfo<TEntity>[]? includeProperties)
        {
            var query  = _dbSet.AsNoTracking().Where(x => x.Id == id);
            if (includeProperties == null) return await query.SingleAsync();
            foreach (var includeProperty in includeProperties)
            {
                var queryWithInclude = query.Include(includeProperty.NavigationProperty);
                queryWithInclude = includeProperty.ThenIncludes.Aggregate(queryWithInclude, (current, thenInclude) => current.ThenInclude(thenInclude));
                query = queryWithInclude;
            }
            return await query.AsSplitQuery().SingleAsync();
        }

        public override void UpdateRange(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreationDate = _timeService.GetCurrentTime();
                entity.CreatedBy = _claimsService.GetCurrentUserId;
            }
            _dbSet.UpdateRange(entities);
        }
    }