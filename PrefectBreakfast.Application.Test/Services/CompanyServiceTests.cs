using System.Linq.Expressions;
using FluentAssertions;
using Hangfire.Server;
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

    
}