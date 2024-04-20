using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Mappers;

public class MapsterConfig : SetupTest
{
    [Fact]
    public void TestUserResponseMapper()
    {
        //arrange
        var mockData = _fixture.Build<User>()
            .Without(x => x.Company)
            .Without(x => x.Delivery)
            .Without(x => x.Partner)
            .Without(x => x.Supplier)
            .Without(x => x.OrdersWorker)
            .Without(x => x.ShippingOrders)
            .Without(x => x.ActionHistories)
            .Without(x => x.UserRoles)
            .Without(x => x.PartnerPayments)
            .Without(x => x.OrdersDeliveryStaff)
            .Create();
        //act
        var result = _mapper.Map<UserResponse>(mockData);
        //assert
        result.Name.Should().Be(mockData.Name.ToString());
    }
    
    [Fact]
    public void TestUserModelResponse()
    {
        //arrange
        var userId = Guid.NewGuid();
        var mockData = new User
        {
            Id = userId,
            Name = "",
            UserRoles = new List<UserRole?>
            {
                new UserRole
                {
                    Role = new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = "admin"
                    },
                    UserId = userId
                }
            }
        };
        //act
        var result = _mapper.Map<UserModelResponse>(mockData);
        //assert
        result.Name.Should().Be(mockData.Name.ToString());
    }
    
}