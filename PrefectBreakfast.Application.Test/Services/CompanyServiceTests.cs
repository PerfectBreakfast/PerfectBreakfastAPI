using System.Linq.Expressions;
using FluentAssertions;
using Hangfire.Server;
using MapsterMapper;
using MockQueryable.Moq;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Request;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.MealModels.Request;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class CompanyServiceTests : SetupTest
{
    private readonly ICompanyService _companyService;

    public CompanyServiceTests()
    {
        _companyService = new CompanyService(
            _unitOfWorkMock.Object,
            _mapper,
            _claimServiceMock.Object,
            _currentTimeMock.Object
        );
    }

    [Fact]
    public async Task CreateCompany_ReturnsSuccess_WithValidRequest()
    {
        // Arrange
        var mockRequest = new CompanyRequest
        {
            Name = "Test Company",
            Meals = new List<MealModel>()
            {
                new MealModel(Guid.NewGuid(), new TimeOnly(10, 0), new TimeOnly(11, 0))
            }
        };

        var mockCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "",
            PhoneNumber = "",
            Email = "null",
            Address = "null"
        };
        var mockMealSubscriptions = new List<MealSubscription>()
        {
            new MealSubscription { CompanyId = mockCompany.Id, MealId = mockRequest.Meals[0].MealId }
        };

        _unitOfWorkMock.Setup(u => u.CompanyRepository.AddAsync(It.IsAny<Company>()))
            .ReturnsAsync(mockCompany);

        _unitOfWorkMock.Setup(u => u.MealSubscriptionRepository.AddRangeAsync(It.IsAny<List<MealSubscription>>()))
            .Returns(Task.CompletedTask);

        /*_mapperMock.Setup(m => m.Map<Company>(mockRequest))
            .Returns(mockCompany);*/

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _companyService.CreateCompany(mockRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCompany_ReturnsError_OnInvalidStartTime()
    {
        // Arrange
        var mockRequest = new CompanyRequest
        {
            Name = "Test Company",
            Meals = new List<MealModel>()
            {
                new MealModel(Guid.NewGuid(), new TimeOnly(11, 0), new TimeOnly(10, 0))
            }
        };

        // Act
        var result = await _companyService.CreateCompany(mockRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e =>
            e.Code == ErrorCode.BadRequest && e.Message.Contains("Giờ bắt đầu phải trước giờ kết thúc"));
    }

    [Fact]
    public async Task CreateCompany_ReturnsError_OnSaveFailure()
    {
        // Arrange
        var mockRequest = new CompanyRequest
        {
            Name = "Test Company",
            Meals = new List<MealModel>()
            {
                new MealModel(Guid.NewGuid(), new TimeOnly(9, 0), new TimeOnly(10, 0))
            }
        };
        var mockCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "",
            PhoneNumber = "",
            Email = "null",
            Address = "null"
        };
        var mockMealSubscriptions = new List<MealSubscription>()
        {
            new MealSubscription { CompanyId = mockCompany.Id, MealId = mockRequest.Meals[0].MealId }
        };

        _unitOfWorkMock.Setup(u => u.CompanyRepository.AddAsync(It.IsAny<Company>()))
            .ReturnsAsync(mockCompany);
        _unitOfWorkMock.Setup(u => u.MealSubscriptionRepository.AddRangeAsync(It.IsAny<List<MealSubscription>>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ReturnsAsync(0);

        // Act
        var result = await _companyService.CreateCompany(mockRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.ServerError);
    }

    [Fact]
    public async Task CreateCompany_ReturnsError_OnException()
    {
        // Arrange
        var mockRequest = new CompanyRequest
        {
            Name = "Test Company",
            Meals = new List<MealModel>()
            {
                new MealModel(Guid.NewGuid(), new TimeOnly(9, 0), new TimeOnly(10, 0))
            }
        };

        _unitOfWorkMock.Setup(u => u.CompanyRepository.AddAsync(It.IsAny<Company>()))
            .ThrowsAsync(new Exception("Error creating company"));

        // Act
        var result = await _companyService.CreateCompany(mockRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.Code == ErrorCode.UnknownError && e.Message.Contains("Error creating company"));
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsSuccess_WithEmptyList()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.CompanyRepository.FindAll(It.IsAny<Expression<Func<Company, bool>>>()))
            .Returns(new List<Company>().AsQueryable().BuildMock());

        // Act
        var result = await _companyService.GetAllCompanies();

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsSuccess_WithPopulatedList()
    {
        // Arrange
        var mockCompanies = new List<Company>()
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company 1",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company 2",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            }
        };

        _unitOfWorkMock.Setup(u => u.CompanyRepository.FindAll(It.IsAny<Expression<Func<Company, bool>>>()))
            .Returns(mockCompanies.AsQueryable().BuildMock());

        /*_mapperMock.Setup(m => m.Map<List<CompanyResponse>>(It.IsAny<List<Company>>()))
            .Returns(new List<CompanyResponse>() { new CompanyResponse(), new CompanyResponse() });*/

        // Act
        var result = await _companyService.GetAllCompanies();

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsError_OnException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.CompanyRepository.FindAll(It.IsAny<Expression<Func<Company, bool>>>()))
            .Throws(new Exception("Error retrieving companies"));

        // Act
        var result = await _companyService.GetAllCompanies();

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e =>
            e.Code == ErrorCode.UnknownError && e.Message.Contains("Error retrieving companies"));
    }

    [Fact]
    public async Task GetUsersByCompanyId_ReturnsSuccess_WithEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByCompanyId(companyId))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _companyService.GetUsersByCompanyId(companyId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetUsersByCompanyId_ReturnsSuccess_WithPopulatedList()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockUsers = new List<User>()
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "test@email.com",
                Name = "Test User",
                CompanyId = companyId,
                UserRoles = new List<UserRole>()
                {
                    new UserRole { Role = new Role { Name = "Admin" } }
                }
            }
        };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByCompanyId(companyId))
            .ReturnsAsync(mockUsers);

        /*_mapperMock.Setup(m => m.Map<List<UserModelResponse>>(It.IsAny<List<User>>()))
            .Returns(new List<UserModelResponse>() { new UserModelResponse() });*/

        // Act
        var result = await _companyService.GetUsersByCompanyId(companyId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Count.Should().Be(1);
        result.Payload.First().Roles.Should().ContainSingle(r => r == "Admin");
    }

    [Fact]
    public async Task GetUsersByCompanyId_ReturnsError_OnNotFound()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByCompanyId(companyId))
            .ThrowsAsync(new NotFoundIdException("Company not found"));

        // Act
        var result = await _companyService.GetUsersByCompanyId(companyId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound && e.Message == "Company not found");
    }

    [Fact]
    public async Task GetUsersByCompanyId_ReturnsError_OnException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByCompanyId(companyId))
            .ThrowsAsync(new Exception("Error retrieving users"));

        // Act
        var result = await _companyService.GetUsersByCompanyId(companyId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e =>
            e.Code == ErrorCode.UnknownError && e.Message.Contains("Error retrieving users"));
    }

    [Fact]
    public async Task SearchCompany_ReturnsSuccess_WithEmptyList()
    {
        // Arrange
        var searchTerm = "non-existent company";
        var now = DateTime.UtcNow;
        _unitOfWorkMock.Setup(u => u.CompanyRepository.SearchCompany(searchTerm, now))
            .ReturnsAsync((List<Company>)null);

        // Act
        var result = await _companyService.SearchCompany(searchTerm);

        // Assert
        result.Payload.Should().BeNull();
        result.Errors.Should()
            .ContainSingle(e => e.Code == ErrorCode.BadRequest && e.Message == "Không có công ty hợp lệ");
    }

    /*[Fact]
    public async Task SearchCompany_ReturnsSuccess_WithPopulatedList()
    {
        // Arrange
        var searchTerm = "Test";
        var now = DateTime.UtcNow.AddHours(7);
        var mockCompanies = new List<Company>()
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Test Company 1",
                CreationDate = now.AddDays(-2),
                PhoneNumber = "null",
                Email = "",
                Address = "null"
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Another Test Company",
                CreationDate = now.AddDays(-3),
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            }
        };

        _unitOfWorkMock.Setup(u => u.CompanyRepository.SearchCompany(searchTerm, now))
            .ReturnsAsync(mockCompanies);

        /*_mapperMock.Setup(m => m.Map<List<CompanyResponse>>(It.IsAny<List<Company>>()))
            .Returns(new List<CompanyResponse>() { new CompanyResponse(), new CompanyResponse() });#1#

        // Act
        var result = await _companyService.SearchCompany(searchTerm);

        // Assert
        result.Payload.Should().NotBeNull();
        result.Payload.Count.Should().Be(2);
    }

    [Fact]
    public async Task SearchCompany_ReturnsError_OnException()
    {
        // Arrange
        var searchTerm = "test";
        var now = DateTime.UtcNow;
        _unitOfWorkMock.Setup(u => u.CompanyRepository.SearchCompany(searchTerm, now))
            .ThrowsAsync(new Exception("Error searching companies"));

        // Act
        var result = await _companyService.SearchCompany(searchTerm);

        // Assert
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message.Contains("Error searching companies"));
    }*/

    [Fact]
    public async Task DeleteCompany_ReturnsSuccess_WithDeletedCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockCompany = new Company
        {
            Id = companyId,
            Name = "null",
            PhoneNumber = "null",
            Email = "null",
            Address = "null"
        };

        _unitOfWorkMock.Setup(u => u.CompanyRepository.GetByIdAsync(companyId))
            .ReturnsAsync(mockCompany);

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _companyService.DeleteCompany(companyId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteCompany_ReturnsError_OnNotFound()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.CompanyRepository.GetByIdAsync(companyId))
            .ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _companyService.DeleteCompany(companyId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound && e.Message == "Id is not exits");
    }

    [Fact]
    public async Task DeleteCompany_ReturnsError_OnException()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.CompanyRepository.GetByIdAsync(companyId))
            .ThrowsAsync(new Exception("Error deleting company"));

        // Act
        var result = await _companyService.DeleteCompany(companyId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e =>
            e.Code == ErrorCode.UnknownError && e.Message.Contains("Error deleting company"));
    }

    [Fact]
    public async Task GetCompany_ReturnsSuccess_WithPopulatedData()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockCompany = new Company
        {
            Id = companyId,
            Name = "Test Company",
            Partner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = "Test Partner",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            },
            Delivery = new Delivery
            {
                Id = Guid.NewGuid(),
                Name = "Test Delivery",
                Address = "",
                PhoneNumber = "",
                CommissionRate = 0
            },
            MealSubscriptions = new List<MealSubscription>()
            {
                new MealSubscription
                {
                    Meal = new Meal
                    {
                        Id = Guid.NewGuid(),
                        MealType = "Lunch"
                    }
                }
            },
            Workers = new List<User>()
            {
                new User
                {
                    Name = ""
                }
            },
            PhoneNumber = "",
            Email = "null",
            Address = "null"
        };

        _unitOfWorkMock.Setup(u => u.CompanyRepository.GetCompanyDetailMealById(companyId))
            .ReturnsAsync(mockCompany);

        /*_mapperMock.Setup(m => m.Map<CompanyResponse>(It.IsAny<Company>()))
            .Returns(new CompanyResponse());

        _mapperMock.Setup(m => m.Map<PartnerResponseModel>(It.IsAny<Partner>()))
            .Returns(new PartnerResponseModel());

        _mapperMock.Setup(m => m.Map<DeliveryResponseModel>(It.IsAny<Delivery>()))
            .Returns(new DeliveryResponseModel());*/

        // Act
        var result = await _companyService.GetCompany(companyId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.Id.Should().Be(companyId);
        result.Payload.Partner.Should().NotBeNull();
        result.Payload.Delivery.Should().NotBeNull();
        result.Payload.MemberCount.Should().Be(1);
        result.Payload.Meals.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetCompany_ReturnsError_OnNotFound()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.CompanyRepository.GetCompanyDetailMealById(companyId))
            .ReturnsAsync((Company)null);

        // Act
        var result = await _companyService.GetCompany(companyId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound && e.Message == "Id is not exist");
    }

    [Fact]
    public async Task GetCompany_ReturnsError_OnException()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.CompanyRepository.GetCompanyDetailMealById(companyId))
            .ThrowsAsync(new Exception("Error retrieving company"));

        // Act
        var result = await _companyService.GetCompany(companyId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        result.Errors.Should().ContainSingle(e =>
            e.Code == ErrorCode.UnknownError && e.Message.Contains("Error retrieving company"));
    }

    [Fact]
    public async Task GetCompanyPaginationAsync_Success_ReturnsFirstPage()
    {
        // Arrange
        var companies = new List<Company>()
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company A",
                Workers = new List<User>()
                {
                    new User
                    {
                        Name = ""
                    }
                },
                PhoneNumber = "null",
                Email = "null",
                Address = "null",
                Partner = new Partner
                {
                    Name = "Partner 1",
                    Address = "null",
                    PhoneNumber = "null",
                    CommissionRate = 0
                },
                Delivery = new Delivery
                {
                    Name = "Delivery 1",
                    Address = "null",
                    PhoneNumber = "null",
                    CommissionRate = 0
                },
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company B",
                Partner = new Partner
                {
                    Name = "Partner 1",
                    Address = "null",
                    PhoneNumber = "null",
                    CommissionRate = 0
                },
                Delivery = new Delivery
                {
                    Name = "Delivery 1",
                    Address = "null",
                    PhoneNumber = "null",
                    CommissionRate = 0
                },
                PhoneNumber = "null",
                Email = "null",
                Address = "null",
                Workers = new List<User>()
                {
                    new User
                    {
                        Name = "1"
                    }
                },
            }
        };
        var expectedCompanyResponse = new List<CompanyResponsePaging>()
        {
            new CompanyResponsePaging
            {
                Id = companies[0].Id,
                Name = companies[0].Name,
                MemberCount = companies[0].Workers.Count,
                ManagementUnit = null,
                DeliveryUnit = null
            }
        };
        _unitOfWorkMock.Setup(r => r.CompanyRepository.ToPagination(0, 10, It.IsAny<Expression<Func<Company, bool>>>(),
                It.IsAny<IncludeInfo<Company>>(), It.IsAny<IncludeInfo<Company>>(), It.IsAny<IncludeInfo<Company>>()))
            .ReturnsAsync(new Pagination<Company>
                { PageIndex = 0, PageSize = 10, TotalItemsCount = companies.Count, Items = companies });
        //_unitOfWorkMock.Setup(u => u.CompanyRepository).Returns(mockCompanyRepo.Object);


        // Act
        var result = await _companyService.GetCompanyPaginationAsync(null);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.PageIndex.Should().Be(0);
        result.Payload.PageSize.Should().Be(10);
        result.Payload.TotalItemsCount.Should().Be(companies.Count);
        //result.Payload.Items.Should().BeEquivalentTo(expectedCompanyResponse);
    }

    [Fact]
    public async Task GetCompanyByPartner_Success_ReturnsFirstPage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var companies = new List<Company>()
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company A",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company B",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            }
        };
        var user = new User
        {
            Id = userId, Partner = new Partner
            {
                Id = partnerId,
                Companies = companies,
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };
        var expectedCompanyResponse = new List<CompanyResponsePaging>()
        {
            new CompanyResponsePaging { Id = companies[0].Id, Name = companies[0].Name },
            new CompanyResponsePaging { Id = companies[1].Id, Name = companies[1].Name }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u =>
                u.CompanyRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Company, bool>>>()))
            .ReturnsAsync(new Pagination<Company>
                { PageIndex = 0, PageSize = 10, TotalItemsCount = companies.Count, Items = companies });
        //mockMapper.Setup(m => m.Map<List<CompanyResponsePaging>>(It.IsAny<List<Company>>())).Returns(expectedCompanyResponse);


        // Act
        var result = await _companyService.GetCompanyByPartner(null);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.PageIndex.Should().Be(0);
        result.Payload.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetCompanyByPartner_SearchTerm_ReturnsFiltered()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var companies = new List<Company>()
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company A",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company Matching",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            }
        };
        var user = new User
        {
            Id = userId, Partner = new Partner
            {
                Id = partnerId,
                Companies = companies,
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };
        var expectedCompanyResponse = new List<CompanyResponsePaging>()
        {
            new CompanyResponsePaging { Id = companies[1].Id, Name = companies[1].Name }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u =>
                u.CompanyRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Company, bool>>>()))
            .ReturnsAsync(new Pagination<Company>
                { PageIndex = 0, PageSize = 10, TotalItemsCount = 1, Items = new List<Company>() { companies[1] } });
        /*mockMapper.Setup(m => m.Map<List<CompanyResponsePaging>>(It.IsAny<List<Company>>()))
            .Returns(expectedCompanyResponse);*/


        // Act
        var result = await _companyService.GetCompanyByPartner("matching");

        // Assert
        result.IsError.Should().BeFalse();
        //result.Payload.Should
    }

    [Fact]
    public async Task GetCompanyByPartner_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _companyService.GetCompanyByPartner(null);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound && e.Message == "User is not exist");
    }

    [Fact]
    public async Task GetCompanyByPartner_NoCompanies_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var user = new User
        {
            Id = userId, Partner = new Partner
            {
                Id = partnerId,
                Companies = new List<Company>(),
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u =>
                u.CompanyRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Company, bool>>>()))
            .ReturnsAsync(new Pagination<Company>
                { PageIndex = 0, PageSize = 10, TotalItemsCount = 0, Items = new List<Company>() });


        // Act
        var result = await _companyService.GetCompanyByPartner(null);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.TotalItemsCount.Should().Be(0);
        result.Payload.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCompanyByPartner_ExceptionDuringPagination_ReturnsUnknownError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var user = new User
        {
            Id = userId, Partner = new Partner
            {
                Id = partnerId,
                Companies = new List<Company>(),
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u =>
                u.CompanyRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Company, bool>>>()))
            .ThrowsAsync(new Exception("Test Exception"));

        // Act
        var result = await _companyService.GetCompanyByPartner(null);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == "Test Exception");
    }

    [Fact]
    public async Task GetCompanyByDelivery_Success_ReturnsFirstPage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deliveryId = Guid.NewGuid();
        var companies = new List<Company>()
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company A",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company B",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            }
        };
        var user = new User
        {
            Id = userId, Delivery = new Delivery
            {
                Id = deliveryId,
                Companies = companies,
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };
        var expectedCompanyResponse = new List<CompanyResponsePaging>()
        {
            new CompanyResponsePaging { Id = companies[0].Id, Name = companies[0].Name },
            new CompanyResponsePaging { Id = companies[1].Id, Name = companies[1].Name }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u =>
                u.CompanyRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Company, bool>>>()))
            .ReturnsAsync(new Pagination<Company>
                { PageIndex = 0, PageSize = 10, TotalItemsCount = companies.Count, Items = companies });
        //mockMapper.Setup(m => m.Map<List<CompanyResponsePaging>>(It.IsAny<List<Company>>())).Returns(expectedCompanyResponse);


        // Act
        var result = await _companyService.GetCompanyByDelivery(null);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.PageIndex.Should().Be(0);
        result.Payload.PageSize.Should().Be(10);
        result.Payload.TotalItemsCount.Should().Be(companies.Count);
    }

    [Fact]
    public async Task GetCompanyByDelivery_SearchTerm_ReturnsFiltered()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deliveryId = Guid.NewGuid();
        var companies = new List<Company>()
        {
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company A",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            },
            new Company
            {
                Id = Guid.NewGuid(),
                Name = "Company Matching",
                PhoneNumber = "null",
                Email = "null",
                Address = "null"
            }
        };
        var user = new User
        {
            Id = userId, Delivery = new Delivery
            {
                Id = deliveryId,
                Companies = companies,
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };
        var expectedCompanyResponse = new List<CompanyResponsePaging>()
        {
            new CompanyResponsePaging { Id = companies[1].Id, Name = companies[1].Name }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u =>
                u.CompanyRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Company, bool>>>()))
            .ReturnsAsync(new Pagination<Company>
                { PageIndex = 0, PageSize = 10, TotalItemsCount = 1, Items = new List<Company>() { companies[1] } });
        /*mockMapper.Setup(m => m.Map<List<CompanyResponsePaging>>(It.IsAny<List<Company>>()))
            .Returns(expectedCompanyResponse);*/


        // Act
        var result = await _companyService.GetCompanyByDelivery("matching");

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task GetCompanyByDelivery_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ThrowsAsync(new NotFoundIdException());


        // Act
        var result = await _companyService.GetCompanyByDelivery(null);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound && e.Message == "User is not exist");
    }

    [Fact]
    public async Task GetCompanyByDelivery_NoCompanies_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deliveryId = Guid.NewGuid();
        var user = new User
        {
            Id = userId, Delivery = new Delivery
            {
                Id = deliveryId,
                Companies = new List<Company>(),
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u =>
                u.CompanyRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Company, bool>>>()))
            .ReturnsAsync(new Pagination<Company>
                { PageIndex = 0, PageSize = 10, TotalItemsCount = 0, Items = new List<Company>() });


        // Act
        var result = await _companyService.GetCompanyByDelivery(null);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
        result.Payload.TotalItemsCount.Should().Be(0);
        result.Payload.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCompanyByDelivery_ExceptionDuringPagination_ReturnsUnknownError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deliveryId = Guid.NewGuid();
        var user = new User
        {
            Id = userId, Delivery = new Delivery
            {
                Id = deliveryId,
                Companies = new List<Company>(),
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };

        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u =>
                u.CompanyRepository.ToPagination(It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<Company, bool>>>()))
            .ThrowsAsync(new Exception("Test Exception"));

        // Act
        var result = await _companyService.GetCompanyByDelivery(null);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == "Test Exception");
    }

    [Fact]
    public async Task UpdateCompany_Success_UpdatesCompanyAndMealSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var existingCompany = new Company
        {
            Id = companyId,
            Name = "Company A",
            PhoneNumber = "null",
            Email = "null",
            Address = "null"
        };
        var existingMealSubscriptions = new List<MealSubscription>()
        {
            new MealSubscription
            {
                Id = Guid.NewGuid(), 
                MealId = Guid.NewGuid(), 
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(11, 0)
            }
        };
        existingCompany.MealSubscriptions = existingMealSubscriptions;
        var updateRequest = new UpdateCompanyRequest
        {
            Name = "Updated Company Name",
            Address = "Updated Address",
            PhoneNumber = "1234567890",
            Email = "updated@company.com",
            Meals = new List<MealModel>()
            {
                new MealModel(existingMealSubscriptions[0].MealId.Value, new TimeOnly(9, 30), new TimeOnly(10, 30)),
                new MealModel(Guid.NewGuid(), new TimeOnly(11, 0), new TimeOnly(12, 0))
            }
        };

        _unitOfWorkMock.Setup(u => u.CompanyRepository.GetByIdAsync(companyId, m => m.MealSubscriptions))
            .ReturnsAsync(existingCompany);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _companyService.UpdateCompany(companyId, updateRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().BeTrue();

        existingCompany.Name.Should().Be(updateRequest.Name);
        existingCompany.Address.Should().Be(updateRequest.Address);
        existingCompany.PhoneNumber.Should().Be(updateRequest.PhoneNumber);
        existingCompany.Email.Should().Be(updateRequest.Email);

        existingCompany.MealSubscriptions.Count.Should().Be(2);

        var updatedMealSubscription =
            existingCompany.MealSubscriptions.FirstOrDefault(ms => ms.MealId == existingMealSubscriptions[0].MealId);
        updatedMealSubscription.Should().NotBeNull();
        updatedMealSubscription.StartTime.Should().Be(updateRequest.Meals[0].StartTime);
        updatedMealSubscription.EndTime.Should().Be(updateRequest.Meals[0].EndTime);

        var newMealSubscription =
            existingCompany.MealSubscriptions.FirstOrDefault(ms => ms.MealId == updateRequest.Meals[1].MealId);
        newMealSubscription.Should().NotBeNull();
        newMealSubscription.StartTime.Should().Be(updateRequest.Meals[1].StartTime);
        newMealSubscription.EndTime.Should().Be(updateRequest.Meals[1].EndTime);
    }
    
    [Fact]
    public async Task UpdateCompany_ExceptionDuringSave_ReturnsUnknownError()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var existingCompany = new Company
        {
            Id = companyId,
            Name = "Company A",
            PhoneNumber = "null",
            Email = "null",
            Address = "null"
        };
        var existingMealSubscriptions = new List<MealSubscription>()
        {
            new MealSubscription
            {
                Id = Guid.NewGuid(), 
                MealId = Guid.NewGuid(), 
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(11, 0)
            }
        };
        existingCompany.MealSubscriptions = existingMealSubscriptions;
        var updateRequest = new UpdateCompanyRequest
        {
            Name = "Updated Company Name",
            Address = "Updated Address",
            PhoneNumber = "1234567890",
            Email = "updated@company.com",
            Meals = new List<MealModel>()
            {
                new MealModel(existingMealSubscriptions[0].MealId.Value, new TimeOnly(9, 30), new TimeOnly(10, 30)),
                new MealModel(Guid.NewGuid(), new TimeOnly(11, 0), new TimeOnly(12, 0))
            }
        };
        
        _unitOfWorkMock.Setup(u => u.CompanyRepository.GetByIdAsync(companyId, m => m.MealSubscriptions))
            .ReturnsAsync(existingCompany);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ThrowsAsync(new Exception("Test Exception"));

        // Act
        var result = await _companyService.UpdateCompany(companyId, updateRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == "Test Exception");
    }
}