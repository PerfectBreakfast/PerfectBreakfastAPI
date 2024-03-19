using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.Repositories;

    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
        }

        public async Task<Category> GetCategoryDetail(Guid id, FoodStatus status)
        {
            var category = await _dbSet.Where(x => x.Id == id)
                .Include(x => x.Foods.Where(x => x.FoodStatus == status))
                .FirstAsync();
            return category;
        }
    }

