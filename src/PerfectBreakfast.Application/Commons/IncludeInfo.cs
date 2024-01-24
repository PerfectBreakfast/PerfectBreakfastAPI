using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Commons;

public class IncludeInfo<TEntity>
{
    public Expression<Func<TEntity, object>> NavigationProperty { get; set; }
    public List<Expression<Func<object, object>>> ThenIncludes { get; set; } = new List<Expression<Func<object, object>>>();
}