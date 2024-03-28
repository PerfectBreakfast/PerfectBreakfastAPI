using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IFoodRepository : IGenericRepository<Food>
    {
        Task<List<Food>> GetFoodForSupplier(Guid id);
    }
}
