using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class CompanyRepositoryTests : SetupTest
{
    private readonly ICompanyRepository _companyRepository;
    public CompanyRepositoryTests()
    {
        _companyRepository = new CompanyRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetCompanyById_ReturnsCorrectCompany()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var company = _fixture.Build<Company>()
            .With(x => x.Id,expectedId)
            .Without(x => x.MealSubscriptions)
            .Without(x => x.Delivery)
            .Without(x => x.Partner)
            .Without(x => x.Workers)
            .Create();
        _dbContext.Companies.Add(company);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _companyRepository.GetCompanyById(expectedId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedId);
    }

    [Fact]
    public async Task GetCompanyDetailMealById_ReturnsCompanyWithDetails()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company
        {
            Id = companyId,
            Partner = new Partner
            {
                Name = "",
                Address = "",
                PhoneNumber = "",
                CommissionRate = 0
            },
            Workers = new List<User>(),
            Delivery = new Delivery
            {
                Name = "",
                Address = "",
                PhoneNumber = "",
                CommissionRate = 0
            },
            MealSubscriptions = new List<MealSubscription>
            {
                new MealSubscription
                {
                    Meal = new Meal
                    {
                        MealType = ""
                    }
                }
            },
            Name = "",
            PhoneNumber = "",
            Email = "",
            Address = ""
        };

        _dbContext.Companies.Add(company);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _companyRepository.GetCompanyDetailMealById(companyId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(companyId);
        result.MealSubscriptions.Should().NotBeEmpty();
        result.Partner.Should().NotBeNull();
        result.Delivery.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchCompany_ReturnsFilteredCompanies()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var pastCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Old Company",
            CreationDate = now.AddDays(-2),
            PhoneNumber = "",
            Email = "",
            Address = ""
        };
        var recentCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Recent Company",
            CreationDate = now.AddDays(-1),
            PhoneNumber = "",
            Email = "",
            Address = ""
        };
        _dbContext.Companies.AddRange(pastCompany, recentCompany);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _companyRepository.SearchCompany("company", now);

        // Assert
        result.Should().HaveCount(2);
    }
}