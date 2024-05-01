using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Services;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class SupplierFoodAssignmentServiceTests : SetupTest
{
    private readonly ISupplierFoodAssignmentService _supplierFoodAssignmentService;

    public SupplierFoodAssignmentServiceTests()
    {
        _supplierFoodAssignmentService = new SupplierFoodAssignmentService(_unitOfWorkMock.Object,
            _mapper, _currentTimeMock.Object, _foodServiceMock.Object, _claimServiceMock.Object,
            _backgroundJobClientMock.Object);
    }
    
    
}