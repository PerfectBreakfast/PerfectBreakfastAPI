using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class CategoryRepositoryTests : SetupTest
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryRepositoryTests()
    {
        _categoryRepository = new CategoryRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    
}