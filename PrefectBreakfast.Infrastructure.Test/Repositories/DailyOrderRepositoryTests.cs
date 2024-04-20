using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class DailyOrderRepositoryTests : SetupTest
{
    private readonly IDailyOrderRepository _dailyOrderRepository;

    public DailyOrderRepositoryTests()
    {
        _dailyOrderRepository = new DailyOrderRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }

    [Fact]
    public async Task FindByBookingDate_ReturnsCorrectlyFilteredOrders()
    {
        // Arrange
        var expectedDate = DateTime.Today.AddDays(1);
        var orders = new List<DailyOrder>
        {
            new DailyOrder
            {
                Id = Guid.NewGuid(), BookingDate = DateOnly.FromDateTime(expectedDate),
                Status = DailyOrderStatus.Initial
            },
            new DailyOrder
            {
                Id = Guid.NewGuid(), BookingDate = DateOnly.FromDateTime(expectedDate.AddDays(-1)),
                Status = DailyOrderStatus.Initial
            },
            new DailyOrder
            {
                Id = Guid.NewGuid(), BookingDate = DateOnly.FromDateTime(expectedDate),
                Status = DailyOrderStatus.Processing
            }
        };

        await _dbContext.DailyOrders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _dailyOrderRepository.FindByBookingDate(DateTime.Now);

        // Assert
        result.Should().HaveCount(1);
        result.All(o => o.BookingDate == DateOnly.FromDateTime(expectedDate) && o.Status == DailyOrderStatus.Initial)
            .Should().BeTrue();
    }

    [Fact]
    public async Task FindPartnerAndDeliveryAdminEmailsByBookingDateAndStatus_ReturnsCorrectEmails()
    {
        // Arrange
        var expectedDate = DateTime.Today.AddDays(1);
        var users = new List<User>
        {
            new User
            {
                Email = "admin1@example.com", Name = "",
                UserRoles = new List<UserRole>
                    { new UserRole { Role = new Role { Name = ConstantRole.PARTNER_ADMIN } } }
            },
            new User
            {
                Email = "admin2@example.com", Name = "",
                UserRoles = new List<UserRole>
                    { new UserRole { Role = new Role { Name = ConstantRole.DELIVERY_ADMIN } } }
            }
        };

        var mealSubscription = new MealSubscription
        {
            Company = new Company
            {
                Partner = new Partner
                {
                    Users = users,
                    Name = string.Empty,
                    Address = string.Empty,
                    PhoneNumber = string.Empty,
                    CommissionRate = 0
                },
                Delivery = new Delivery
                {
                    Users = users,
                    Name = string.Empty,
                    Address = string.Empty,
                    PhoneNumber = string.Empty,
                    CommissionRate = 0
                },
                Name = string.Empty,
                PhoneNumber = string.Empty,
                Email = string.Empty,
                Address = string.Empty
            }
        };
        var orders = new List<DailyOrder>
        {
            new DailyOrder
            {
                BookingDate = DateOnly.FromDateTime(expectedDate), Status = DailyOrderStatus.Processing,
                MealSubscription = mealSubscription
            }
        };

        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.DailyOrders.AddRangeAsync(orders);
        await _dbContext.SaveChangesAsync();

        // Act
        var result =
            await _dailyOrderRepository.FindPartnerAndDeliveryAdminEmailsByBookingDateAndStatus(DateTime.Today);

        // Assert
        result.Should().HaveCount(2).And.Contain(new[] { "admin1@example.com", "admin2@example.com" });
    }

    [Fact]
    public async Task IsDailyOrderCreated_ReturnsTrueIfOrderExists()
    {
        // Arrange
        var targetDate = DateTime.Today.AddDays(2); // Adjusting for method implementation details
        var order = new DailyOrder
            { BookingDate = DateOnly.FromDateTime(targetDate), Status = DailyOrderStatus.Initial };
        await _dbContext.DailyOrders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _dailyOrderRepository.IsDailyOrderCreated(DateTime.Today);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByMeal_ReturnsFilteredDailyOrdersWithIncludes()
    {
        // Arrange
        var mealSubscriptionId1 = Guid.NewGuid();
        var mealSubscriptionId2 = Guid.NewGuid(); // Will not be used in the query to test filtering.
        var fromDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var toDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var mealSubscriptions = new List<MealSubscription>
        {
            new MealSubscription
            {
                Id = mealSubscriptionId1, Company = new Company
                {
                    Partner = new Partner
                    {
                        Name = string.Empty,
                        Address = string.Empty,
                        PhoneNumber = string.Empty,
                        CommissionRate = 0
                    },
                    Delivery = new Delivery
                    {
                        Name = string.Empty,
                        Address = string.Empty,
                        PhoneNumber = string.Empty,
                        CommissionRate = 0
                    },
                    Name = string.Empty,
                    PhoneNumber = string.Empty,
                    Email = string.Empty,
                    Address = string.Empty
                },
                Meal = new Meal
                {
                    MealType = string.Empty
                }
            },
            new MealSubscription { Id = mealSubscriptionId2 } // Not used in filtering.
        };

        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId1,
                MealSubscription = mealSubscriptions[0],
                BookingDate = DateOnly.FromDateTime(DateTime.Today),
                Orders = new List<Order> { new Order() }
            },
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId1,
                MealSubscription = mealSubscriptions[0],
                BookingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), // Outside of date range.
                Orders = new List<Order> { new Order() }
            }
        };

        _dbContext.MealSubscriptions.AddRange(mealSubscriptions);
        _dbContext.DailyOrders.AddRange(dailyOrders);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _dailyOrderRepository.GetByMeal(new List<Guid> { mealSubscriptionId1 }, fromDate, toDate);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(d =>
            d.MealSubscriptionId == mealSubscriptionId1 && d.BookingDate == DateOnly.FromDateTime(DateTime.Today));
        result.First().MealSubscription.Should().NotBeNull();
        result.First().MealSubscription.Company.Should().NotBeNull();
        result.First().MealSubscription.Company.Partner.Should().NotBeNull();
        result.First().MealSubscription.Company.Delivery.Should().NotBeNull();
        result.First().MealSubscription.Meal.Should().NotBeNull();
        result.First().Orders.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetForStatistic_ReturnsOrdersWithinDateRangeWithPositiveOrderQuantity()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5));
        var toDate = DateOnly.FromDateTime(DateTime.Today);

        var meal = new Meal
        {
            Id = Guid.NewGuid(),
            MealType = string.Empty
        };
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Partner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                Address = string.Empty,
                PhoneNumber = string.Empty,
                CommissionRate = 0
            },
            Delivery = new Delivery
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                Address = string.Empty,
                PhoneNumber = string.Empty,
                CommissionRate = 0
            },
            Name = string.Empty,
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Address = string.Empty
        };
        var mealSubscription = new MealSubscription
        {
            Id = Guid.NewGuid(),
            Company = company,
            Meal = meal
        };

        var orders = new List<DailyOrder>
        {
            new DailyOrder
            {
                Id = Guid.NewGuid(),
                BookingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                OrderQuantity = 10,
                MealSubscription = mealSubscription,
                Orders = new List<Order> { new Order { Id = Guid.NewGuid() } }
            },
            new DailyOrder
            {
                Id = Guid.NewGuid(),
                BookingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), // Outside of date range
                OrderQuantity = 5,
                MealSubscription = mealSubscription,
                Orders = new List<Order> { new Order { Id = Guid.NewGuid() } }
            },
            new DailyOrder
            {
                Id = Guid.NewGuid(),
                BookingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)),
                OrderQuantity = 0, // No order quantity
                MealSubscription = mealSubscription,
                Orders = new List<Order> { new Order { Id = Guid.NewGuid() } }
            }
        };

        _dbContext.AddRange(meal, company, mealSubscription);
        _dbContext.AddRange(orders);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _dailyOrderRepository.GetForStatistic(fromDate, toDate);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().ContainSingle()
            .Which.OrderQuantity.Should().BeGreaterThan(0);
        result.Should().ContainSingle()
            .Which.BookingDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));
        result.Should().ContainSingle()
            .Which.MealSubscription.Should().NotBeNull();
        result.Single().MealSubscription.Company.Should().NotBeNull();
        result.Single().MealSubscription.Company.Partner.Should().NotBeNull();
        result.Single().MealSubscription.Company.Delivery.Should().NotBeNull();
        result.Single().MealSubscription.Meal.Should().NotBeNull();
        result.Single().Orders.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ToPaginationProcessingForPartner_ReturnsCorrectPaginationData()
    {
        // Arrange
        var mealSubscriptionId1 = Guid.NewGuid();
        var mealSubscriptionId2 = Guid.NewGuid();

        var meal = new Meal
        {
            Id = Guid.NewGuid(),
            MealType = string.Empty
        };
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Address = string.Empty
        };

        var relevantOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId1,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId1, Meal = meal, Company = company },
                BookingDate = DateOnly.FromDateTime(DateTime.Now), OrderQuantity = 5,
                Status = DailyOrderStatus.Processing
            },
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId2,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId2, Meal = meal, Company = company },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), OrderQuantity = 3,
                Status = DailyOrderStatus.Processing
            }
        };

        await _dbContext.AddRangeAsync(relevantOrders);
        await _dbContext.SaveChangesAsync();

        int pageNumber = 0;
        int pageSize = 10;

        // Act
        var result =
            await _dailyOrderRepository.ToPaginationProcessingForPartner(new List<Guid> { mealSubscriptionId1 },
                pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.PageIndex.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalItemsCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.All(o => o.MealSubscriptionId == mealSubscriptionId1).Should().BeTrue();
        result.Items.Should().OnlyContain(o => o.Status == DailyOrderStatus.Processing);
        result.Items.Should().OnlyContain(o =>
            o.MealSubscription != null && o.MealSubscription.Company != null && o.MealSubscription.Meal != null);
        result.Items.Should().BeInDescendingOrder(o => o.BookingDate);
    }


    [Fact]
    public async Task ToPaginationForDeliveryDistribution_ReturnsCorrectPaginationData()
    {
        // Arrange
        var mealSubscriptionId1 = Guid.NewGuid();
        var mealSubscriptionId2 = Guid.NewGuid();
        var mealSubscriptionId3 = Guid.NewGuid();

        var meal = new Meal
        {
            Id = Guid.NewGuid(),
            MealType = "Special Meal",
        };
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Special Company",
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Address = string.Empty
        };

        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId1,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId1, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                OrderQuantity = 10,
                Status = DailyOrderStatus.Divided // Status that should be included
            },
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId2,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId2, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                OrderQuantity = 0,
                Status = DailyOrderStatus.Initial // Should not be included
            },
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId3,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId3, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                OrderQuantity = 5,
                Status = DailyOrderStatus.Delivering // Should not be included
            }
        };

        await _dbContext.AddRangeAsync(dailyOrders);
        await _dbContext.SaveChangesAsync();

        int pageNumber = 0;
        int pageSize = 1; // Set to 1 to test pagination

        // Act
        var result =
            await _dailyOrderRepository.ToPaginationForDeliveryDistribution(
                new List<Guid> { mealSubscriptionId1, mealSubscriptionId2, mealSubscriptionId3 }, pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.PageIndex.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalItemsCount.Should().Be(1); // Only one order meets the criteria
        result.Items.Should().HaveCount(1);
        result.Items.Should().BeInDescendingOrder(o => o.BookingDate);
    }

    [Fact]
    public async Task ToPaginationForDelivery_ReturnsCorrectPaginationData()
    {
        // Arrange
        var mealSubscriptionId1 = Guid.NewGuid();
        var mealSubscriptionId2 = Guid.NewGuid();
        var mealSubscriptionId3 = Guid.NewGuid();

        var meal = new Meal { Id = Guid.NewGuid(), MealType = "Special Meal" };
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Special Company",
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Address = string.Empty
        };

        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId1,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId1, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                OrderQuantity = 10,
                Status = DailyOrderStatus.Complete // Should be included
            },
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId2,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId2, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                OrderQuantity = 10,
                Status = DailyOrderStatus.Delivering // Should be included
            },
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId3,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId3, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                OrderQuantity = 0,
                Status = DailyOrderStatus.Complete // Should not be included due to zero order quantity
            }
        };

        await _dbContext.AddRangeAsync(dailyOrders);
        await _dbContext.SaveChangesAsync();

        int pageNumber = 0;
        int pageSize = 2; // Set to 2 to verify correct pagination

        // Act
        var result = await _dailyOrderRepository.ToPaginationForDelivery(
            new List<Guid> { mealSubscriptionId1, mealSubscriptionId2, mealSubscriptionId3 }, pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.PageIndex.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalItemsCount.Should().Be(2); // Only two orders meet the criteria
        result.Items.Should().HaveCount(2);
        result.Items.Should().Match(b =>
            b.All(o => o.Status == DailyOrderStatus.Complete || o.Status == DailyOrderStatus.Delivering));
        result.Items.Should().Match(b => b.All(o => o.OrderQuantity > 0));
        result.Items.Should().ContainSingle(o => o.Status == DailyOrderStatus.Complete);
        result.Items.Should().ContainSingle(o => o.Status == DailyOrderStatus.Delivering);
        result.Items.Should().OnlyContain(o =>
            o.MealSubscription != null && o.MealSubscription.Company != null && o.MealSubscription.Meal != null);
        result.Items.Should().BeInDescendingOrder(o => o.BookingDate);
    }

    [Fact]
    public async Task ToPaginationForPartner_ReturnsCorrectPaginationData()
    {
        // Arrange
        var mealSubscriptionId1 = Guid.NewGuid();
        var mealSubscriptionId2 =
            Guid.NewGuid(); // This ID should not affect the result since no orders are associated with it in this test.

        var meal = new Meal { Id = Guid.NewGuid(), MealType = "Healthy Meal" };
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Healthy Foods Co.",
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Address = string.Empty
        };

        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId1,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId1, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                OrderQuantity = 10,
                Status = DailyOrderStatus.Delivering
            },
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId2,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId2, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                OrderQuantity = 0, // Should not be included due to zero order quantity
                Status = DailyOrderStatus.Processing
            }
        };

        await _dbContext.AddRangeAsync(dailyOrders);
        await _dbContext.SaveChangesAsync();

        int pageNumber = 0;
        int pageSize = 1; // Using a small pageSize to test pagination

        // Act
        var result =
            await _dailyOrderRepository.ToPaginationForPartner(
                new List<Guid> { mealSubscriptionId1, mealSubscriptionId2 }, pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.PageIndex.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalItemsCount.Should().Be(1); // Only one order meets the criteria
        result.Items.Should().HaveCount(1);
        result.Items.Should().BeInDescendingOrder(o => o.BookingDate);
    }

    [Fact]
    public async Task ToPagination_ReturnsCorrectPaginationData()
    {
        // Arrange
        var mealSubscriptionId1 = Guid.NewGuid();
        var mealSubscriptionId2 = Guid.NewGuid();

        var meal = new Meal { Id = Guid.NewGuid(), MealType = "Healthy Meal" };
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Healthy Foods Co.",
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Address = string.Empty
        };

        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId1,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId1, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                OrderQuantity = 10,
                Status = DailyOrderStatus.Complete // This order should be included when filtering by Status.Complete
            },
            new DailyOrder
            {
                MealSubscriptionId = mealSubscriptionId2,
                MealSubscription = new MealSubscription { Id = mealSubscriptionId2, Company = company, Meal = meal },
                BookingDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                OrderQuantity = 5,
                Status = DailyOrderStatus.Delivering // This order should be included only when not filtering by status
            }
        };

        await _dbContext.AddRangeAsync(dailyOrders);
        await _dbContext.SaveChangesAsync();

        int pageNumber = 0;
        int pageSize = 1; // Small pageSize to test pagination

        // Act - Test without status filter
        var resultNoStatus =
            await _dailyOrderRepository.ToPagination(new List<Guid> { mealSubscriptionId1,mealSubscriptionId2 }, null, pageNumber,
                pageSize);
        var resultWithStatus = await _dailyOrderRepository.ToPagination(new List<Guid> { mealSubscriptionId1,mealSubscriptionId2 },
            DailyOrderStatus.Complete, pageNumber, pageSize);

        // Assert - No status filter
        resultNoStatus.Should().NotBeNull();
        resultNoStatus.PageIndex.Should().Be(pageNumber);
        resultNoStatus.PageSize.Should().Be(pageSize);
        resultNoStatus.TotalItemsCount.Should().Be(2); // Both orders should be included
        resultNoStatus.Items.Should().HaveCount(1); // Because of pageSize

        // Assert - With status filter
        resultWithStatus.Should().NotBeNull();
        resultWithStatus.PageIndex.Should().Be(pageNumber);
        resultWithStatus.PageSize.Should().Be(pageSize);
        resultWithStatus.TotalItemsCount.Should().Be(1); // Only one order meets the status criteria
        resultWithStatus.Items.Should().HaveCount(1);
        resultWithStatus.Items.Should().BeInDescendingOrder(o => o.BookingDate);
    }
}