using AutoFixture;
using Moq;
using PerfectBreakfast.Application.Interfaces;

namespace PrefectBreakfast.Domain.Test;

public class SetupTest : IDisposable
{
    protected readonly Fixture _fixture;
    protected readonly Mock<IUnitOfWork> __unitOfWorkMock;
    protected readonly Mock<ICategoryService> _categoryServiceMock;
    
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}