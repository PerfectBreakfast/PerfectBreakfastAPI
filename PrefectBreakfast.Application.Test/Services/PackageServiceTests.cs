using Moq;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class PackageServiceTests : SetupTest
{
    private readonly IPackageService _packageService;

    public PackageServiceTests()
    {
        _packageService = new PackageService(_unitOfWorkMock.Object, _mapper, _claimServiceMock.Object, _currentTimeMock.Object);
    }

    
}
