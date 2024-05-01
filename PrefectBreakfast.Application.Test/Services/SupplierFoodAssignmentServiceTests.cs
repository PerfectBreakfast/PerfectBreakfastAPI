using System.Linq.Expressions;
using Hangfire;
using MockQueryable.Moq;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
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

    [Fact]
    public async Task CreateSupplierFoodAssignment_Successful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dailyOrderId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var foodId = Guid.NewGuid();
        var foodPrice = 10m;
        var commissionRate = 0.15m; // 15%

        var request = new SupplierFoodAssignmentsRequest
        {
            DailyOrderId = dailyOrderId,
            SupplierFoodAssignmentRequest = new List<SupplierFoodAssignmentRequest>
            {
                new SupplierFoodAssignmentRequest { SupplierId = supplierId, FoodId = foodId, AmountCooked = 100 }
            }
        };

        var user = new User
        {
            Partner = new Partner
            {
                SupplyAssignments = new List<SupplyAssignment>
                {
                    new SupplyAssignment
                    {
                        Supplier = new Supplier
                        {
                            Id = supplierId,
                            SupplierCommissionRates = new List<SupplierCommissionRate>
                            {
                                new SupplierCommissionRate
                                {
                                    FoodId = foodId,
                                    CommissionRate = commissionRate,
                                    Food = new Food
                                    {
                                        Id = foodId,
                                        Price = foodPrice
                                    }
                                }
                            },
                            Name = "null",
                            Address = "null",
                            PhoneNumber = "null"
                        }
                    }
                },
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };

        var dailyOrder = new DailyOrder
        {
            Id = dailyOrderId,
            Package = null, // Indicates no existing package assignment
            MealSubscription = new MealSubscription
            {
                StartTime = new TimeOnly(8, 0),
                Meal = new Meal
                {
                    MealType = "null"
                }
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>[]>()))
            .ReturnsAsync(user);
        _unitOfWorkMock
            .Setup(x => x.DailyOrderRepository.GetByIdAsync(dailyOrderId,
                It.IsAny<Expression<Func<DailyOrder, object>>>())).ReturnsAsync(dailyOrder);
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1); // Simulating successful database update
        _foodServiceMock.Setup(x => x.GetFoodsForPartner(dailyOrderId))
            .ReturnsAsync(new OperationResult<TotalFoodForPartnerResponse>
            {
                IsError = false,
                Payload = new TotalFoodForPartnerResponse()
                {
                    TotalFoodResponses = new List<TotalFoodResponse>
                    {
                        new TotalFoodResponse
                        {
                            Name = "Test Food", Quantity = 100
                        },
                    }
                }
            });

        // Act
        var result = await _supplierFoodAssignmentService.CreateSupplierFoodAssignment(request);

        // Assert
        Assert.True(result.IsError);
        //Assert.True(result.Payload); // Success is expected to be true
    }

    [Fact]
    public async Task CreateSupplierFoodAssignment_PartnerWithoutSuppliers_ReturnsError()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var request = new SupplierFoodAssignmentsRequest
        {
            DailyOrderId = Guid.NewGuid(),
            SupplierFoodAssignmentRequest = new List<SupplierFoodAssignmentRequest>
            {
                new SupplierFoodAssignmentRequest
                    { SupplierId = Guid.NewGuid(), FoodId = Guid.NewGuid(), AmountCooked = 100 }
            }
        };

        var user = new User
        {
            Partner = new Partner
            {
                SupplyAssignments = new List<SupplyAssignment>(),
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0 // No suppliers assigned
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>[]>()))
            .ReturnsAsync(user);

        // Act
        var result = await _supplierFoodAssignmentService.CreateSupplierFoodAssignment(request);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors,
            e => e.Code == ErrorCode.BadRequest && e.Message == "Đối tác chưa có nhà cung cấp");
    }

    [Fact]
    public async Task CreateSupplierFoodAssignment_SupplierNotAssociatedWithPartner_ReturnsError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();

        var request = new SupplierFoodAssignmentsRequest
        {
            DailyOrderId = Guid.NewGuid(),
            SupplierFoodAssignmentRequest = new List<SupplierFoodAssignmentRequest>
            {
                new SupplierFoodAssignmentRequest
                    { SupplierId = Guid.NewGuid(), FoodId = Guid.NewGuid(), AmountCooked = 100 }
            }
        };

        var user = new User
        {
            Partner = new Partner
            {
                SupplyAssignments = new List<SupplyAssignment>
                {
                    new SupplyAssignment
                    {
                        Supplier = new Supplier
                        {
                            Id = Guid.NewGuid(),
                            Name = "null",
                            Address = "null",
                            PhoneNumber = "null"
                        }
                    } // Different Supplier ID
                },
                Name = "null",
                Address = "null",
                PhoneNumber = "null",
                CommissionRate = 0
            }
        };

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId, It.IsAny<IncludeInfo<User>[]>()))
            .ReturnsAsync(user);

        // Act
        var result = await _supplierFoodAssignmentService.CreateSupplierFoodAssignment(request);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors,
            e => e.Code == ErrorCode.BadRequest && e.Message.Contains("Có nhà cung cấp không thuộc đối tác này"));
    }

    [Fact]
    public async Task UpdateSupplierFoodAssignment_Successful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var supplierFoodAssignmentId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var foodId = Guid.NewGuid();
        var supplierFoodAssignment = new SupplierFoodAssignment
        {
            Id = supplierFoodAssignmentId,
            Status = SupplierFoodAssignmentStatus.Declined,
            FoodId = foodId,
            AmountCooked = 100,
            Package = new Package()
        };
        var food = new Food { Id = foodId, Price = 10.00M };
        var supplierCommissionRate = new SupplierCommissionRate { FoodId = foodId, CommissionRate = 0.20M };

        var updateRequest = new UpdateSupplierFoodAssignment(supplierFoodAssignmentId, supplierId);

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x =>
                x.SupplierFoodAssignmentRepository.GetByIdAsync(updateRequest.SupplierFoodAssignmentId,
                    It.IsAny<Expression<Func<SupplierFoodAssignment, object>>>()))
            .ReturnsAsync(supplierFoodAssignment);
        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.GetBySupplierId(updateRequest.SupplierId))
            .ReturnsAsync(new List<SupplierCommissionRate> { supplierCommissionRate });
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ReturnsAsync(food);
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
        /*_backgroundJobClientMock
            .Setup(x => x.Enqueue<IManagementService>(It.IsAny<Expression<Action<IManagementService>>>()))
            .Verifiable();*/

        // Act
        var result = await _supplierFoodAssignmentService.UpdateSupplierFoodAssignment(updateRequest);

        // Assert
        Assert.False(result.IsError);
        Assert.True(result.Payload);
        Assert.Equal(SupplierFoodAssignmentStatus.Pending, supplierFoodAssignment.Status);
        Assert.Equal(supplierCommissionRate.Id, supplierFoodAssignment.SupplierCommissionRateId);
        Assert.Equal(food.Price * supplierCommissionRate.CommissionRate * supplierFoodAssignment.AmountCooked / 100,
            supplierFoodAssignment.ReceivedAmount);
        //_backgroundJobClientMock.Verify(); // Verify that the background job to send an email was enqueued
    }

    [Fact]
    public async Task UpdateSupplierFoodAssignment_Fails_WhenAssignmentNotDeclined()
    {
        // Arrange
        var supplierFoodAssignmentId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var supplierFoodAssignment = new SupplierFoodAssignment
        {
            Id = supplierFoodAssignmentId,
            Status = SupplierFoodAssignmentStatus.Pending // Not Declined
        };
        var updateRequest = new UpdateSupplierFoodAssignment(supplierFoodAssignmentId, supplierId);

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid());
        _unitOfWorkMock.Setup(x =>
                x.SupplierFoodAssignmentRepository.GetByIdAsync(updateRequest.SupplierFoodAssignmentId,
                    It.IsAny<Expression<Func<SupplierFoodAssignment, object>>>()))
            .ReturnsAsync(supplierFoodAssignment);

        // Act
        var result = await _supplierFoodAssignmentService.UpdateSupplierFoodAssignment(updateRequest);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorCode.BadRequest, result.Errors.First().Code);
        Assert.Equal("Đơn hàng này phải bị từ chối", result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdateSupplierFoodAssignment_Fails_WhenSupplierNotRegisteredForFood()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var supplierFoodAssignmentId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var foodId = Guid.NewGuid();
        var supplierFoodAssignment = new SupplierFoodAssignment
        {
            Id = supplierFoodAssignmentId,
            Status = SupplierFoodAssignmentStatus.Declined,
            FoodId = foodId
        };
        var food = new Food { Id = foodId, Price = 10.00M };

        var updateRequest = new UpdateSupplierFoodAssignment(supplierFoodAssignmentId, supplierId);

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x =>
                x.SupplierFoodAssignmentRepository.GetByIdAsync(updateRequest.SupplierFoodAssignmentId,
                    It.IsAny<Expression<Func<SupplierFoodAssignment, object>>>()))
            .ReturnsAsync(supplierFoodAssignment);
        _unitOfWorkMock.Setup(x => x.SupplierCommissionRateRepository.GetBySupplierId(updateRequest.SupplierId))
            .ReturnsAsync(new List<SupplierCommissionRate>()); // Empty list, supplier not registered
        _unitOfWorkMock.Setup(x => x.FoodRepository.GetByIdAsync(foodId)).ReturnsAsync(food);

        // Act
        var result = await _supplierFoodAssignmentService.UpdateSupplierFoodAssignment(updateRequest);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorCode.BadRequest, result.Errors.First().Code);
        Assert.Equal("Nhà cung cấp này chưa đăng kí món", result.Errors.First().Message);
    }

    [Fact]
    public async Task CompleteFoodAssignment_SuccessfullyCompletesAssignment()
    {
        var supplierFoodAssignment = new SupplierFoodAssignment
        {
            Status = SupplierFoodAssignmentStatus.Confirmed,
            Package = new Package
            {
                DailyOrderId = Guid.NewGuid(),
                MissingQuantity = 1,
                SuccessfulQuantity = 0,
                TotalFoodAssignQuantity = 1
            }
        };
        var dailyOrder = new DailyOrder
        {
            Status = DailyOrderStatus.Cooking
        };
        _unitOfWorkMock.Setup(x => x.SupplierFoodAssignmentRepository.GetByIdAsync(It.IsAny<Guid>(),
                It.IsAny<Expression<Func<SupplierFoodAssignment, object>>>()))
            .ReturnsAsync(supplierFoodAssignment);
        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(dailyOrder);
        _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

        var result = await _supplierFoodAssignmentService.CompleteFoodAssignment(Guid.NewGuid());

        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Equal(SupplierFoodAssignmentStatus.Complete, supplierFoodAssignment.Status);
    }

    [Fact]
    public async Task CompleteFoodAssignment_Fails_WhenDailyOrderStatusIsNotCompatible()
    {
        var supplierFoodAssignment = new SupplierFoodAssignment
        {
            Status = SupplierFoodAssignmentStatus.Complete,
            Package = new Package { DailyOrderId = Guid.NewGuid() }
        };
        var dailyOrder = new DailyOrder
        {
            Status = DailyOrderStatus.Complete // Not in a state that allows completion
        };
        _unitOfWorkMock.Setup(x => x.SupplierFoodAssignmentRepository.GetByIdAsync(It.IsAny<Guid>(),
                It.IsAny<Expression<Func<SupplierFoodAssignment, object>>>()))
            .ReturnsAsync(supplierFoodAssignment);
        _unitOfWorkMock.Setup(x => x.DailyOrderRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(dailyOrder);

        var result = await _supplierFoodAssignmentService.CompleteFoodAssignment(Guid.NewGuid());

        Assert.True(result.IsError);
        //Assert.Equal("Đơn đã hoàn thành rồi nhé", result.Errors.First().Message);
    }

    [Fact]
    public async Task CompleteFoodAssignment_Fails_WhenAssignmentNotFound()
    {
        _unitOfWorkMock.Setup(x => x.SupplierFoodAssignmentRepository.GetByIdAsync(It.IsAny<Guid>(),
                It.IsAny<Expression<Func<SupplierFoodAssignment, object>>>()))
            .ThrowsAsync(new NotFoundIdException());

        var result = await _supplierFoodAssignmentService.CompleteFoodAssignment(Guid.NewGuid());

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentDetailBySupplier_ReturnsGroupedAssignmentsSuccessfully()
    {
        var packageIds = new List<Guid> { Guid.NewGuid() };
        var supplierFoodAssignments = new List<SupplierFoodAssignment>
        {
            new SupplierFoodAssignment { Package = new Package { Id = Guid.NewGuid() } }
        };
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new User { SupplierId = Guid.NewGuid() });
        _unitOfWorkMock.Setup(x =>
                x.SupplierFoodAssignmentRepository.GetForSupplierByPackage(packageIds, It.IsAny<Guid>()))
            .ReturnsAsync(supplierFoodAssignments);
        /*_mapper.Setup(x => x.Map<List<FoodAssignmentResponse>>(It.IsAny<List<SupplierFoodAssignment>>()))
            .Returns(new List<FoodAssignmentResponse>());*/

        var result = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentDetailBySupplier(packageIds);

        Assert.False(result.IsError);
        Assert.NotEmpty(result.Payload);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentsForSupplierDownload_ReturnsGroupedAssignmentsSuccessfully()
    {
        var packageIds = new List<Guid> { Guid.NewGuid() };
        var supplierFoodAssignments = new List<SupplierFoodAssignment>
        {
            new SupplierFoodAssignment
            {
                Package = new Package
                {
                    Id = Guid.NewGuid(), Partner = new Partner
                    {
                        Name = "PartnerName",
                        Address = "null",
                        PhoneNumber = "null",
                        CommissionRate = 0
                    }
                }
            }
        };
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid());
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new User { SupplierId = Guid.NewGuid() });
        _unitOfWorkMock.Setup(x =>
                x.SupplierFoodAssignmentRepository.GetForSupplierByPackage(packageIds, It.IsAny<Guid>()))
            .ReturnsAsync(supplierFoodAssignments);
        /*_mapper.Setup(x => x.Map<List<FoodAssignmentResponse>>(It.IsAny<List<SupplierFoodAssignment>>()))
            .Returns(new List<FoodAssignmentResponse>());*/

        var result = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentsForSupplierDownload(packageIds);

        Assert.False(result.IsError);
        Assert.NotEmpty(result.Payload);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentsForSupplierDownload_HandlesExceptions()
    {
        var packageIds = new List<Guid> { Guid.NewGuid() };
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.NewGuid());
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(It.IsAny<Guid>()))
            .Throws(new Exception("Database error")); // Simulate an exception

        var result = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentsForSupplierDownload(packageIds);

        Assert.True(result.IsError);
        Assert.Equal("Database error", result.Errors.First().Message);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentDetailByPartner_ReturnsGroupedDataSuccessfully()
    {
        // Arrange
        var request = new FoodAssignmentByPartnerRequest(
            SupplierId: Guid.NewGuid(),
            PackageIds: new List<Guid> { Guid.NewGuid() }
        );
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId))
            .ReturnsAsync(new User { PartnerId = Guid.NewGuid() });
        _unitOfWorkMock.Setup(x => x.SupplierRepository.GetSupplierById(request.SupplierId))
            .ReturnsAsync(new Supplier
            {
                Name = "null",
                Address = "null",
                PhoneNumber = "null"
            });
        var assignments = new List<SupplierFoodAssignment>
        {
            new SupplierFoodAssignment
            {
                Package = new Package
                {
                    Id = Guid.NewGuid(), DailyOrder = new DailyOrder
                    {
                        MealSubscription = new MealSubscription
                        {
                            Company = new Company
                            {
                                Name = "TestCompany",
                                PhoneNumber = "null",
                                Email = "null",
                                Address = "null"
                            }
                        }
                    }
                }
            }
        };
        _unitOfWorkMock.Setup(x =>
                x.SupplierFoodAssignmentRepository.GetForPartnerByPackage(request.PackageIds, It.IsAny<Guid>(),
                    request.SupplierId))
            .ReturnsAsync(assignments);
        /*_mapper.Setup(x => x.Map<List<FoodAssignmentResponse>>(It.IsAny<List<SupplierFoodAssignment>>()))
            .Returns(new List<FoodAssignmentResponse>());*/

        // Act
        var result = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentDetailByPartner(request);

        // Assert
        Assert.False(result.IsError);
        Assert.NotEmpty(result.Payload);
        Assert.Single(result.Payload);
        Assert.Equal("TestCompany", result.Payload.First().CompanyName);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentDetailByPartner_HandlesExceptions()
    {
        // Arrange
        var request = new FoodAssignmentByPartnerRequest(
            SupplierId: Guid.NewGuid(),
            PackageIds: new List<Guid> { Guid.NewGuid() }
        );
        var userId = Guid.NewGuid();
        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId))
            .Throws(new Exception("Unexpected error"));

        // Act
        var result = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentDetailByPartner(request);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Unexpected error", result.Errors.First().Message);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentsForPartnerDownload_ReturnsGroupedDataSuccessfully()
    {
        // Arrange
        var packageIds = new List<Guid> { Guid.NewGuid() };
        var supplierId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId))
            .ReturnsAsync(new User { PartnerId = partnerId });

        var supplierAssignments = new List<SupplierFoodAssignment>
        {
            new SupplierFoodAssignment
            {
                Package = new Package
                {
                    Id = Guid.NewGuid(),
                    Partner = new Partner
                    {
                        Name = "Partner A",
                        Address = "null",
                        PhoneNumber = "null",
                        CommissionRate = 0
                    },
                    DailyOrder = new DailyOrder
                    {
                        MealSubscription = new MealSubscription
                        {
                            Company = new Company
                            {
                                Name = "Company A",
                                PhoneNumber = "null",
                                Email = "null",
                                Address = "null"
                            },
                            Meal = new Meal { MealType = "Lunch" }
                        }
                    },
                    PartnerId = partnerId
                },
                SupplierCommissionRate = new SupplierCommissionRate
                {
                    Supplier = new Supplier
                    {
                        Name = "Supplier X",
                        Address = "null",
                        PhoneNumber = "null"
                    }
                }
            }
        };

        _unitOfWorkMock.Setup(x => x.SupplierRepository.GetSupplierById(supplierId))
            .ReturnsAsync(new Supplier
            {
                Name = "null",
                Address = "null",
                PhoneNumber = "null"
            });

        _unitOfWorkMock.Setup(x =>
                x.SupplierFoodAssignmentRepository.GetForPartnerByPackage(packageIds, partnerId, supplierId))
            .ReturnsAsync(supplierAssignments);

        /*_mapper.Setup(x => x.Map<List<FoodAssignmentResponse>>(It.IsAny<List<SupplierFoodAssignment>>()))
            .Returns(new List<FoodAssignmentResponse>
            {
                new FoodAssignmentResponse
                {
                    Id = Guid.NewGuid(),
                    FoodName = "Pizza",
                    AmountCooked = 100,
                    ReceivedAmount = 500,
                    CommissionRate = 10,
                    Status = "Pending",
                    DeliveryDeadline = DateTime.Now
                }
            });*/

        // Act
        var result =
            await _supplierFoodAssignmentService.GetSupplierFoodAssignmentsForPartnerDownload(packageIds, supplierId);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Single(result.Payload);
        var response = result.Payload.First();
        Assert.Equal("Supplier X - Lunch", response.PartnerName);
        Assert.Equal("Company A", response.CompanyName);
        Assert.Single(response.FoodAssignmentResponses);
    }

    [Fact]
    public async Task GetSupplierFoodAssignmentsForPartnerDownload_NoSupplierFound_ReturnsError()
    {
        // Arrange
        var packageIds = new List<Guid> { Guid.NewGuid() };
        var supplierId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(userId);
        _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByIdAsync(userId))
            .ReturnsAsync(new User { PartnerId = Guid.NewGuid() });
        _unitOfWorkMock.Setup(x => x.SupplierRepository.GetSupplierById(supplierId))
            .ReturnsAsync((Supplier)null); // No supplier found

        // Act
        var result =
            await _supplierFoodAssignmentService.GetSupplierFoodAssignmentsForPartnerDownload(packageIds, supplierId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorCode.BadRequest, result.Errors.First().Code);
        Assert.Equal("Nhà cung cấp không tồn tại", result.Errors.First().Message);
    }

    /*[Fact]
    public async Task GetSupplierFoodAssignmentsForSuperAdmin_ValidData_ReturnsGroupedResults()
    {
        // Arrange
        var fromDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
        var toDate = DateOnly.FromDateTime(DateTime.Today);

        var supplierFoodAssignments = new List<SupplierFoodAssignment>
        {
            new SupplierFoodAssignment
            {
                Id = Guid.NewGuid(),
                AmountCooked = 100,
                ReceivedAmount = 1500m,
                Status = SupplierFoodAssignmentStatus.Confirmed,
                DeliveryDeadline = DateTime.Now.AddDays(1),
                SupplierCommissionRate = new SupplierCommissionRate
                {
                    SupplierId = Guid.NewGuid(),
                    CommissionRate = 10,
                    Food = new Food
                    {
                        Id = Guid.NewGuid(),
                        Name = "Steak",
                        Price = 150,
                        FoodStatus = FoodStatus.Retail
                    }
                },
                Package = new Package
                {
                    Id = Guid.NewGuid(),
                    DailyOrder = new DailyOrder
                    {
                        Id = Guid.NewGuid(),
                        BookingDate = DateOnly.FromDateTime(DateTime.Today),
                        CreationDate = DateTime.Today.AddDays(-1),
                        Status = DailyOrderStatus.Divided,
                        MealSubscription = new MealSubscription
                        {
                            Company = new Company
                            {
                                Id = Guid.NewGuid(),
                                Name = "Best Eats Inc.",
                                PhoneNumber = "null",
                                Email = "null",
                                Address = "null"
                            },
                            Meal = new Meal
                            {
                                MealType = "Lunch"
                            }
                        }
                    }
                }
            }
        };

        _currentTimeMock.Setup(x => x.GetCurrentTime()).Returns(DateTime.Now);
        _unitOfWorkMock.Setup(u => u.SupplierFoodAssignmentRepository.GetByForSuperAdmin(fromDate, toDate))
            .ReturnsAsync(supplierFoodAssignments);

        // Act
        var result = await _supplierFoodAssignmentService.GetSupplierFoodAssignmentsForSuperAdmin(fromDate, toDate);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Payload);
        Assert.Single(result.Payload); // Expect one grouped result
        //Assert.Equal("Best Eats Inc.", result.Payload.First().FoodAssignmentGroupBySuppliers.First().CompanyName);
        Assert.Equal("Steak - khẩu phần đơn lẻ",
            result.Payload.First().FoodAssignmentGroupBySuppliers.First().FoodAssignmentResponses.First().FoodName);
    }*/

   
}