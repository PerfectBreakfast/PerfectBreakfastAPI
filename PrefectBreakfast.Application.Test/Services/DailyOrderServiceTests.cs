using System.Linq.Expressions;
using FluentAssertions.Primitives;
using MockQueryable.Moq;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DailyOrder.Request;
using PerfectBreakfast.Application.Models.DailyOrder.Response;
using PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class DailyOrderServiceTests : SetupTest
{
    private readonly IDailyOrderService _dailyOrderService;

    public DailyOrderServiceTests()
    {
        _dailyOrderService = new DailyOrderService(_unitOfWorkMock.Object, _mapper, _claimServiceMock.Object,
            _currentTimeMock.Object);
    }

    [Fact]
    public async Task CreateDailyOrder_SuccessfullyCreatesOrder()
    {
        // Arrange
        var dailyOrderRequest = new DailyOrderRequest(); // Assuming this is populated appropriately
        var dailyOrder = new DailyOrder { Status = DailyOrderStatus.Initial };
        //_mapperMock.Setup(m => m.Map<DailyOrder>(dailyOrderRequest)).Returns(dailyOrder);
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.AddAsync(dailyOrder)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1); // Simulate successful save
        //_mapperMock.Setup(m => m.Map<DailyOrderResponse>(dailyOrder)).Returns(new DailyOrderResponse()); // Assuming mapping setup

        // Act
        var result = await _dailyOrderService.CreateDailyOrder(dailyOrderRequest);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        _unitOfWorkMock.Verify(u => u.DailyOrderRepository.AddAsync(It.IsAny<DailyOrder>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateDailyOrder_FailsOnSave()
    {
        // Arrange
        var dailyOrderRequest = new DailyOrderRequest();
        var dailyOrder = new DailyOrder();
        //_mapperMock.Setup(m => m.Map<DailyOrder>(dailyOrderRequest)).Returns(dailyOrder);
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.AddAsync(dailyOrder)).Verifiable();
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(0); // Simulate failure to save

        // Act
        var result = await _dailyOrderService.CreateDailyOrder(dailyOrderRequest);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Company or Admin is not exist"));
    }

    [Fact]
    public async Task GetDailyOrders_ReturnsOrdersSuccessfully()
    {
        // Arrange
        var fakeDailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                Id = Guid.NewGuid(), TotalPrice = 100m, OrderQuantity = 10,
                BookingDate = DateOnly.FromDateTime(DateTime.Now), Status = DailyOrderStatus.Complete
            }
        };
        var expectedResponse = new List<DailyOrderResponse>
        {
            new DailyOrderResponse
            {
                Id = fakeDailyOrders[0].Id,
                TotalPrice = fakeDailyOrders[0].TotalPrice,
                OrderQuantity = fakeDailyOrders[0].OrderQuantity,
                BookingDate = fakeDailyOrders[0].BookingDate,
                Status = fakeDailyOrders[0].Status,
                Company = null // Assuming no company is linked in the mock setup
            }
        };

        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetAllAsync())
            .ReturnsAsync(fakeDailyOrders);
        /*_mapperMock.Setup(m => m.Map<List<DailyOrderResponse>>(fakeDailyOrders))
            .Returns(expectedResponse);*/

        // Act
        var result = await _dailyOrderService.GetDailyOrders();

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(expectedResponse.Count, result.Payload.Count);
        Assert.Equal(expectedResponse[0].Id, result.Payload[0].Id);
        Assert.Equal(expectedResponse[0].TotalPrice, result.Payload[0].TotalPrice);
        Assert.Equal(expectedResponse[0].OrderQuantity, result.Payload[0].OrderQuantity);
        Assert.Equal(expectedResponse[0].BookingDate, result.Payload[0].BookingDate);
        Assert.Equal(expectedResponse[0].Status, result.Payload[0].Status);
    }

    [Fact]
    public async Task GetDailyOrders_HandlesExceptions()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetAllAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _dailyOrderService.GetDailyOrders();

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }

    [Fact]
    public async Task GetDailyOrderPaginationAsync_ReturnsPaginatedAndGroupedOrdersSuccessfully()
    {
        // Arrange
        var mealSubscriptions = new List<MealSubscription>
        {
            new MealSubscription
            {
                Id = Guid.NewGuid(), Company = new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Company",
                    Address = "123 Test St",
                    PhoneNumber = "null",
                    Email = "null"
                },
                Meal = new Meal { MealType = "Breakfast" }
            }
        };
        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                Id = Guid.NewGuid(), BookingDate = DateOnly.FromDateTime(DateTime.Now),
                MealSubscription = mealSubscriptions[0], TotalPrice = 100, OrderQuantity = 5,
                Status = DailyOrderStatus.Complete
            }
        };
        var pagination = new Pagination<DailyOrder>
            { Items = dailyOrders, PageIndex = 0, PageSize = 10, TotalItemsCount = dailyOrders.Count };

        _unitOfWorkMock.Setup(u =>
                u.MealSubscriptionRepository.FindAll(It.IsAny<Expression<Func<MealSubscription, object>>>(),
                    It.IsAny<Expression<Func<MealSubscription, object>>>()))
            .Returns(mealSubscriptions.AsQueryable().BuildMock());
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.ToPagination(It.IsAny<List<Guid>>(),
                It.IsAny<DailyOrderStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(pagination);

        // Act
        var result = await _dailyOrderService.GetDailyOrderPaginationAsync(null, 0, 10);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(1, result.Payload.Items.Count);
        var response = result.Payload.Items.First();
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), response.BookingDate);
        Assert.Single(response.Companies);
        Assert.Equal("Test Company", response.Companies.First().Name);
    }

    [Fact]
    public async Task GetDailyOrderPaginationAsync_HandlesException()
    {
        // Arrange
        _unitOfWorkMock.Setup(u =>
                u.MealSubscriptionRepository.FindAll(It.IsAny<Expression<Func<MealSubscription, object>>>(),
                    It.IsAny<Expression<Func<MealSubscription, object>>>()))
            .Throws(new Exception("Database error"));

        // Act
        var result = await _dailyOrderService.GetDailyOrderPaginationAsync(null, 0, 10);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Database error"));
    }

    [Fact]
    public async Task CompleteDailyOrder_SuccessfullyCompletesOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var now = DateTime.Now;
        var dailyOrder = new DailyOrder
        {
            Id = orderId, Status = DailyOrderStatus.Delivering,
            MealSubscription = new MealSubscription { EndTime = TimeOnly.FromDateTime(now.AddHours(-1)) }
        };
        _unitOfWorkMock.Setup(u =>
                u.DailyOrderRepository.GetByIdAndIncludeAsync(orderId, It.IsAny<IncludeInfo<DailyOrder>>()))
            .ReturnsAsync(dailyOrder);
        _currentTimeMock.Setup(c => c.GetCurrentTime()).Returns(now);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _dailyOrderService.CompleteDailyOrder(orderId);

        // Assert
        Assert.False(result.IsError);
        Assert.True(result.Payload);
    }

    [Fact]
    public async Task CompleteDailyOrder_FailsWhenNotDelivering()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var dailyOrder = new DailyOrder { Id = orderId, Status = DailyOrderStatus.Initial };
        _unitOfWorkMock.Setup(u =>
                u.DailyOrderRepository.GetByIdAndIncludeAsync(orderId, It.IsAny<IncludeInfo<DailyOrder>>()))
            .ReturnsAsync(dailyOrder);

        // Act
        var result = await _dailyOrderService.CompleteDailyOrder(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Đơn hàng phải trong quá trình giao"));
    }

    [Fact]
    public async Task CompleteDailyOrder_FailsBeforeEndTime()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var now = DateTime.Now;
        var dailyOrder = new DailyOrder
        {
            Id = orderId, Status = DailyOrderStatus.Delivering,
            MealSubscription = new MealSubscription { EndTime = TimeOnly.FromDateTime(now.AddHours(1)) }
        };
        _unitOfWorkMock.Setup(u =>
                u.DailyOrderRepository.GetByIdAndIncludeAsync(orderId, It.IsAny<IncludeInfo<DailyOrder>>()))
            .ReturnsAsync(dailyOrder);
        _currentTimeMock.Setup(c => c.GetCurrentTime()).Returns(now);

        // Act
        var result = await _dailyOrderService.CompleteDailyOrder(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Chưa tới giờ xác nhận đơn hàng"));
    }

    [Fact]
    public async Task CompleteDailyOrder_HandlesNotFoundException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u =>
                u.DailyOrderRepository.GetByIdAndIncludeAsync(orderId, It.IsAny<IncludeInfo<DailyOrder>>()))
            .Throws(new NotFoundIdException());

        // Act
        var result = await _dailyOrderService.CompleteDailyOrder(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Id is not exist"));
    }

    [Fact]
    public async Task CompleteDailyOrder_HandlesException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u =>
                u.DailyOrderRepository.GetByIdAndIncludeAsync(orderId, It.IsAny<IncludeInfo<DailyOrder>>()))
            .Throws(new Exception("Unexpected error"));

        // Act
        var result = await _dailyOrderService.CompleteDailyOrder(orderId);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Unexpected error"));
    }

    [Fact]
    public async Task GetDailyOrderProcessingByPartner_ReturnsPaginatedResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        var partner = new Partner
        {
            Companies = new List<Company>
            {
                new Company
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                    MealSubscriptions = new List<MealSubscription>
                    {
                        new MealSubscription
                        {
                            Id = Guid.NewGuid(),
                            IsDeleted = false
                        }
                    },
                    Name = "null",
                    PhoneNumber = "null",
                    Email = "null",
                    Address = "null"
                }
            },
            Name = "null",
            Address = "null",
            PhoneNumber = "null",
            CommissionRate = 0
        };
        var user = new User { Partner = partner };
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(user);

        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                Id = Guid.NewGuid(), BookingDate = DateOnly.FromDateTime(DateTime.Now),
                MealSubscription = new MealSubscription
                {
                    CompanyId = partner.Companies.First().Id, Company = partner.Companies.First(),
                    Meal = new Meal { MealType = "Breakfast" }
                },
                TotalPrice = 100, OrderQuantity = 5, Status = DailyOrderStatus.Processing
            }
        };
        var pagination = new Pagination<DailyOrder>
            { Items = dailyOrders, TotalItemsCount = 1, PageIndex = 0, PageSize = 10 };
        _unitOfWorkMock.Setup(u =>
                u.DailyOrderRepository.ToPaginationProcessingForPartner(It.IsAny<List<Guid>>(), It.IsAny<int>(),
                    It.IsAny<int>()))
            .ReturnsAsync(pagination);

        // Act
        var result = await _dailyOrderService.GetDailyOrderProcessingByPartner(0, 10);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(1, result.Payload.TotalItemsCount);
        Assert.Single(result.Payload.Items);
    }

    [Fact]
    public async Task GetDailyOrderProcessingByPartner_HandlesExceptions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<IncludeInfo<User>>()))
            .Throws(new Exception("Failed to retrieve user"));

        // Act
        var result = await _dailyOrderService.GetDailyOrderProcessingByPartner(0, 10);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains("Failed to retrieve user", result.Errors.First().Message);
    }
    

    [Fact]
    public async Task GetDailyOrderByPartner_HandlesExceptions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .Throws(new Exception("Failed to retrieve user"));

        // Act
        var result = await _dailyOrderService.GetDailyOrderByPartner(0, 10);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message.Contains("Failed to retrieve user"));
    }

    [Fact]
    public async Task GetDailyOrderProcessingByDelivery_ReturnsCorrectlyGroupedData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mealSubscriptionIds = new List<Guid> { Guid.NewGuid() };
        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                Id = Guid.NewGuid(), BookingDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                MealSubscription = new MealSubscription
                {
                    Company = new Company
                    {
                        Id = Guid.NewGuid(),
                        Name = "Company A",
                        Address = "Address A",
                        PhoneNumber = "null",
                        Email = "null"
                    },
                    Meal = new Meal { MealType = "Breakfast" }
                },
                TotalPrice = 150, OrderQuantity = 5, Status = DailyOrderStatus.Processing
            }
        };

        var pagination = new Pagination<DailyOrder>
            { Items = dailyOrders, TotalItemsCount = 1, PageIndex = 0, PageSize = 10 };
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(new User
            {
                Delivery = new Delivery
                {
                    Companies = new List<Company>
                    {
                        new Company
                        {
                            MealSubscriptions = new List<MealSubscription>
                            {
                                new MealSubscription
                                {
                                    Id = mealSubscriptionIds.First()
                                }
                            },
                            Name = "null",
                            PhoneNumber = "null",
                            Email = "null",
                            Address = "null"
                        }
                    },
                    Name = "null",
                    Address = "null",
                    PhoneNumber = "null",
                    CommissionRate = 0
                }
            });
        _unitOfWorkMock.Setup(u =>
                u.DailyOrderRepository.ToPaginationForDeliveryDistribution(It.IsAny<List<Guid>>(), 0, 10))
            .ReturnsAsync(pagination);

        // Act
        var result = await _dailyOrderService.GetDailyOrderProcessingByDelivery(0, 10);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(1, result.Payload.TotalItemsCount);
        Assert.Single(result.Payload.Items);
        var responseGroup = result.Payload.Items.First();
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), responseGroup.BookingDate);
        Assert.Single(responseGroup.Companies);
    }

    [Fact]
    public async Task GetDailyOrderProcessingByDelivery_HandlesMissingUserOrCompany()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ThrowsAsync(
                new ArgumentNullException(
                    "User not found or user does not own delivery company")); // Simulate user not found

        // Act
        var result = await _dailyOrderService.GetDailyOrderProcessingByDelivery(0, 10);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message == "User not found or user does not own delivery company");
    }

    [Fact]
    public async Task GetDailyOrderByDelivery_ReturnsPaginatedAndGroupedDataSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mealSubscriptionIds = new List<Guid> { Guid.NewGuid() };
        var dailyOrders = new List<DailyOrder>
        {
            new DailyOrder
            {
                Id = Guid.NewGuid(), BookingDate = DateOnly.FromDateTime(DateTime.UtcNow), MealSubscription =
                    new MealSubscription
                    {
                        Company = new Company
                        {
                            Id = Guid.NewGuid(),
                            Name = "Company A",
                            Address = "Address A",
                            PhoneNumber = "null",
                            Email = "null"
                        },
                        Meal = new Meal { MealType = "Breakfast" }
                    },
                TotalPrice = 150, OrderQuantity = 5, Status = DailyOrderStatus.Delivering
            }
        };

        var pagination = new Pagination<DailyOrder>
            { Items = dailyOrders, TotalItemsCount = 1, PageIndex = 0, PageSize = 10 };
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ReturnsAsync(new User
            {
                Delivery = new Delivery
                {
                    Companies = new List<Company>
                    {
                        new Company
                        {
                            MealSubscriptions = new List<MealSubscription>
                            {
                                new MealSubscription
                                {
                                    Id = mealSubscriptionIds.First()
                                }
                            },
                            Name = "null",
                            PhoneNumber = "null",
                            Email = "null",
                            Address = "null"
                        }
                    },
                    Name = "null",
                    Address = "null",
                    PhoneNumber = "null",
                    CommissionRate = 0
                }
            });
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.ToPaginationForDelivery(It.IsAny<List<Guid>>(), 0, 10))
            .ReturnsAsync(pagination);

        // Act
        var result = await _dailyOrderService.GetDailyOrderByDelivery(0, 10);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(1, result.Payload.TotalItemsCount);
        Assert.Single(result.Payload.Items);
        var responseGroup = result.Payload.Items.First();
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), responseGroup.BookingDate);
        Assert.Single(responseGroup.Companies);
    }

    [Fact]
    public async Task GetDailyOrderByDelivery_HandlesMissingUserOrCompany()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(c => c.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>>()))
            .ThrowsAsync(new ArgumentNullException("User not found or user does not own delivery company"));

        // Act
        var result = await _dailyOrderService.GetDailyOrderByDelivery(0, 10);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Message == "User not found or user does not own delivery company");
    }

    /*[Fact]
    public async Task GetDailyOrderForDownload_UserIsPartnerAdmin_ReturnsCorrectData()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.Now);
        var userId = Guid.NewGuid();

        var mockUser = new User
        {
            Id = userId,
            Partner = new Partner
            {
                Companies = new List<Company>
                {
                    new Company
                    {
                        Id = Guid.NewGuid(),
                        MealSubscriptions = new List<MealSubscription>
                        {
                            new MealSubscription
                            {
                                Meal = new Meal
                                {
                                    MealType = "Lunch"
                                },
                                DailyOrders = new List<DailyOrder>
                                {
                                    new DailyOrder
                                    {
                                        BookingDate = DateOnly.FromDateTime(DateTime.Now),
                                        Status = DailyOrderStatus.Complete,
                                        OrderQuantity = 10,
                                        TotalPrice = 100
                                    }
                                }
                            }
                        },
                        Name = "null",
                        PhoneNumber = "null",
                        Email = "null",
                        Address = "null"
                    }
                },
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>[]>()))
            .ReturnsAsync(mockUser);
        /*_unitOfWorkMock.Setup(u => u.UserManager.IsInRoleAsync(mockUser, ConstantRole.PARTNER_ADMIN))
            .ReturnsAsync(true);#1#
        _unitOfWorkMock.Setup(u => u.DailyOrderRepository.GetByMeal(It.IsAny<List<Guid>>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>()))
            .ReturnsAsync(mockUser.Partner.Companies.SelectMany(c => c.MealSubscriptions.SelectMany(ms => ms.DailyOrders)).ToList());

        // Expected response setup
        var expectedResponse = new List<DailyOrderStatisticResponse>
        {
            // Mock response data based on the entity setup
        };

        /*_mapperMock.Setup(m => m.Map<List<DailyOrderStatisticResponse>>(It.IsAny<List<DailyOrder>>()))
            .Returns(expectedResponse);#1#

        // Act
        var result = await _dailyOrderService.GetDailyOrderForDownload(fromDate, toDate);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(expectedResponse, result.Payload);
    }*/
}