using FluentAssertions;
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
        _packageService = new PackageService(_unitOfWorkMock.Object, _mapper, _claimServiceMock.Object,
            _currentTimeMock.Object);
    }

    [Fact]
    public async Task GetPackageForPartner_Success_ReturnsPackagesGroupedByBookingDateAndCompany()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var companyA = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Company A",
            PhoneNumber = "",
            Email = "null",
            Address = "null"
        };
        var companyB = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Company B",
            PhoneNumber = "null",
            Email = "null",
            Address = "null"
        };
        var mealA = new Meal { Id = Guid.NewGuid(), MealType = "Lunch" };
        var mealB = new Meal { Id = Guid.NewGuid(), MealType = "Dinner" };
        var today = _currentTimeMock.Object.GetCurrentTime().Date;
        var yesterday = today.AddDays(-1);

        var user = new User { Id = userId, PartnerId = partnerId };
        var dailyOrder1 = new DailyOrder
            { BookingDate =  DateOnly.FromDateTime(today), MealSubscription = new MealSubscription { Company = companyA, Meal = mealA } };
        var dailyOrder2 = new DailyOrder
            { BookingDate = DateOnly.FromDateTime(today), MealSubscription = new MealSubscription { Company = companyB, Meal = mealB } };
        var dailyOrder3 = new DailyOrder
            { BookingDate = DateOnly.FromDateTime(today), MealSubscription = new MealSubscription { Company = companyA, Meal = mealA } };
        var package1 = new Package { DailyOrder = dailyOrder1 };
        var package2 = new Package { DailyOrder = dailyOrder2 };
        var package3 = new Package { DailyOrder = dailyOrder3 };

        _claimServiceMock.Setup(s => s.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.PackageRepository.GetByPartner(partnerId))
            .ReturnsAsync(new List<Package>() { package1, package2, package3 });
        /*mockMapper.Setup(m => m.Map<List<PackageForMeal>>(It.IsAny<List<Package>>()))
            .Returns(new List<PackageForMeal>()); // Mock mapping*/

        // Act
        var result = await _packageService.GetPackageForPartner();

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();

        // Assert for today's packages
        var todayPackage = result.Payload.First();
        todayPackage.BookingDate.Should().Be(DateOnly.FromDateTime(today));

        // Assert for company A on today
        var companyAPackage = todayPackage.CompanyForPackages.First(c => c.CompanyName == companyA.Name);
        companyAPackage.MealForCompanies.Count.Should().Be(1); // One meal type (Lunch)
        
    }
}