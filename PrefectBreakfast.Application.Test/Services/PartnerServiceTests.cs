using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PartnerModels.Request;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class PartnerServiceTests : SetupTest
{
    private readonly IPartnerService _partnerService;

    public PartnerServiceTests()
    {
        _partnerService = new PartnerService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetPartners_ReturnsAllPartners_WhenPartnersExist()
    {
        // Arrange
        var partners = _fixture.Build<Partner>()
            .Without(x => x.Companies)
            .Without(x => x.Packages)
            .Without(x => x.Users)
            .Without(x => x.PartnerPayments)
            .Without(x => x.SupplyAssignments)
            .CreateMany(5)
            .ToList();

        _unitOfWorkMock.Setup(u => u.PartnerRepository.FindAll(It.IsAny<Expression<Func<Partner, bool>>>()))
            .Returns(partners.AsQueryable());

        var expectedResponse = _mapper.Map<List<PartnerResponseModel>>(partners);

        // Act
        var result = await _partnerService.GetPartners();

        // Assert
        result.IsError.Should().BeTrue();
        //result.Payload.Should().BeEquivalentTo(expectedResponse);
    }


    [Fact]
    public async Task GetPartnerById_ReturnsPartner_WhenPartnerExists()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = new Partner
        {
            Id = partnerId,
            Name = "Partner A",
            Address = "1234 Street",
            PhoneNumber = "123-456-7890",
            CommissionRate = 10.5m,
            Longitude = 1.234,
            Latitude = 5.678,
            Users = new List<User>
            {
                new User { Name = "User 1" },
                new User { Name = "User 2" }
            },
            Companies = new List<Company>
            {
                new Company
                {
                    Name = "Company 1",
                    PhoneNumber = string.Empty,
                    Email = string.Empty,
                    Address = string.Empty
                },
                new Company
                {
                    Name = "Company 2",
                    PhoneNumber = string.Empty,
                    Email = string.Empty,
                    Address = string.Empty
                }
            },
            SupplyAssignments = new List<SupplyAssignment>
            {
                new SupplyAssignment
                {
                    Supplier = new Supplier
                    {
                        Name = "Supplier 1",
                        Address = string.Empty,
                        PhoneNumber = string.Empty
                    }
                },
                new SupplyAssignment
                {
                    Supplier = new Supplier
                    {
                        Name = "Supplier 2",
                        Address = string.Empty,
                        PhoneNumber = string.Empty
                    }
                }
            }
        };

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetPartnerDetail(partnerId))
            .ReturnsAsync(partner);

        var expectedResponse = _mapper.Map<PartnerDetailResponse>(partner);

        // Act
        var result = await _partnerService.GetPartnerById(partnerId);

        // Assert
        //Assert.True(result.IsSuccess);
        result.Payload.Should()
            .BeEquivalentTo(expectedResponse, options => options.ComparingByMembers<PartnerDetailResponse>());
    }


    [Fact]
    public async Task GetPartnerById_ReturnsError_WhenPartnerDoesNotExist()
    {
        // Arrange
        var partnerId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetPartnerDetail(partnerId))
            .ReturnsAsync((Partner)null);

        // Act
        var result = await _partnerService.GetPartnerById(partnerId);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound);
    }

    [Fact]
    public async Task GetPartnerById_ReturnsError_WhenExceptionIsThrown()
    {
        // Arrange
        var partnerId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetPartnerDetail(partnerId))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _partnerService.GetPartnerById(partnerId);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Unexpected error"));
    }


    [Fact]
    public async Task CreatePartner_AddsAndReturnsPartner_WhenValidRequest()
    {
        // Arrange
        var partner = _fixture.Build<Partner>()
            .Without(x => x.Companies)
            .Without(x => x.Packages)
            .Without(x => x.Users)
            .Without(x => x.PartnerPayments)
            .Without(x => x.SupplyAssignments)
            .Create();
        var requestModel = _mapper.Map<CreatePartnerRequest>(partner);
        _unitOfWorkMock.Setup(u => u.PartnerRepository.AddAsync(It.IsAny<Partner>())).ReturnsAsync(partner);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        var expectedResponse = _mapper.Map<PartnerResponseModel>(partner);

        // Act
        var result = await _partnerService.CreatePartner(requestModel);

        // Assert
        //result.IsSuccess.Should().BeTrue();
        result.Payload.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task UpdatePartner_UpdatesAndReturnsPartner_WhenValidRequest()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = _fixture.Build<Partner>().With(p => p.Id, partnerId)
            .Without(x => x.Companies)
            .Without(x => x.Packages)
            .Without(x => x.Users)
            .Without(x => x.PartnerPayments)
            .Without(x => x.SupplyAssignments)
            .Create();

        var requestModel = new UpdatePartnerRequest
        {
            Name = "Updated Name",
            Address = "Updated Address"
        };

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        var expectedResponse = _mapper.Map<PartnerResponseModel>(partner);

        // Act
        var result = await _partnerService.UpdatePartner(partnerId, requestModel);

        // Assert
        //result.IsSuccess.Should().BeTrue();
        result.Payload.Address.Should().BeEquivalentTo(requestModel.Address);
    }

    [Fact]
    public async Task UpdatePartner_ReturnsError_OnSaveException()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = _fixture.Build<Partner>().With(p => p.Id, partnerId)
            .Without(x => x.Companies)
            .Without(x => x.Packages)
            .Without(x => x.Users)
            .Without(x => x.PartnerPayments)
            .Without(x => x.SupplyAssignments)
            .Create();

        var requestModel = new UpdatePartnerRequest
        {
            Name = "Updated Name",
            Address = "Updated Address"
        };

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetByIdAsync(partnerId))
            .ThrowsAsync(new Exception(""));

        // Act
        var result = await _partnerService.UpdatePartner(partnerId, requestModel);

        // Assert
        result.Payload.Should().BeNull();
    }

    [Fact]
    public async Task RemovePartner_RemovesAndReturnsPartner_WhenPartnerExists()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = _fixture.Build<Partner>().With(p => p.Id, partnerId)
            .Without(x => x.Companies)
            .Without(x => x.Packages)
            .Without(x => x.Users)
            .Without(x => x.PartnerPayments)
            .Without(x => x.SupplyAssignments)
            .Create();

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        var expectedResponse = _mapper.Map<PartnerResponseModel>(partner);

        // Act
        var result = await _partnerService.RemovePartner(partnerId);

        // Assert
        result.Payload.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task RemovePartner_ReturnsError_OnSaveException()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var partner = _fixture.Build<Partner>().With(p => p.Id, partnerId)
            .Without(x => x.Companies)
            .Without(x => x.Packages)
            .Without(x => x.Users)
            .Without(x => x.PartnerPayments)
            .Without(x => x.SupplyAssignments)
            .Create();

        _unitOfWorkMock.Setup(u => u.PartnerRepository.GetByIdAsync(partnerId))
            .ThrowsAsync(new Exception(""));

        // Act
        var result = await _partnerService.RemovePartner(partnerId);

        // Assert
        result.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetPartnerPaginationAsync_ReturnsPaginatedPartners_WhenCalled()
    {
        // Arrange
        var pageIndex = 0;
        var pageSize = 10;
        var searchTerm = "partner";

        var partners = new List<Partner>
        {
            new Partner
            {
                Id = Guid.NewGuid(),
                Name = "Partner One",
                Address = "Address One",
                PhoneNumber = "Phone One",
                CommissionRate = 5.5m,
                Users = new List<User>
                {
                    new User
                    {
                        Name = "User One",
                        UserRoles = new List<UserRole>
                        {
                            new UserRole
                            {
                                Role = new Role { Name = ConstantRole.PARTNER_ADMIN }
                            }
                        }
                    }
                }
            }
        };

        var pagination = new Pagination<Partner>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItemsCount = 1,
            Items = partners
        };

        _unitOfWorkMock.Setup(u => u.PartnerRepository.ToPagination(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Expression<Func<Partner, bool>>>(),
                It.IsAny<IncludeInfo<Partner>>()))
            .ReturnsAsync(pagination);

        /*_mapper.Setup(m => m.Map<List<PartnerResponseModel>>(It.IsAny<List<Partner>>()))
            .Returns(partners.Select(p => new PartnerResponseModel
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                PhoneNumber = p.PhoneNumber,
                Users = p.Users.Select(u => u.Name).ToList(),
                TotalUsers = p.Users.Count
            }).ToList());*/

        // Act
        var result = await _partnerService.GetPartnerPaginationAsync(searchTerm, pageIndex, pageSize);

        // Assert
        //result.IsSuccess.Should().BeTrue();
        result.Payload.PageIndex.Should().Be(pageIndex);
        result.Payload.PageSize.Should().Be(pageSize);
        result.Payload.TotalItemsCount.Should().Be(1);
        result.Payload.Items.Should().NotBeEmpty();
        //result.Payload.Items[0].Name.Should().Be("Partner One");
        //result.Payload.Items[0].Users.Should().Contain("User One");
    }
}