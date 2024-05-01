using Microsoft.AspNetCore.Mvc;
using Moq;
using PerfectBreakfast.API.Controllers.V1;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.PackageModels.Response;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.API.Test.Controller;

public class PackageControllerTest : SetupTest
{
    private PackageController _packageController;

    public PackageControllerTest()
    {
        _packageController = new PackageController(_packageServiceMock.Object);
    }
    
    [Fact]
    public async Task GetPackageForPartner_ReturnsOk_WhenNoErrors()
    {
        // Arrange
        var mockResponse = new OperationResult<List<PackageResponse>>
        {
            Payload = new List<PackageResponse>
            {
               
            },
            IsError = false
        };
        _packageServiceMock.Setup(x => x.GetPackageForPartner()).ReturnsAsync(mockResponse);

        // Act
        var result = await _packageController.GetPackageForPartner();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResponse.Payload, okResult.Value);
    }


}