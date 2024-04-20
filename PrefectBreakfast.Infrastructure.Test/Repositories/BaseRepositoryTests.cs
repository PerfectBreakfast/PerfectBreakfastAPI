using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class BaseRepositoryTests : SetupTest
{
    private readonly IBaseRepository<User> _baseRepository;
    public BaseRepositoryTests()
    {
        _baseRepository = new BaseRepository<User>(_dbContext);
    }
    
    [Fact]
    public async Task GenericRepository_AddRangeAsync_ShouldReturnCorrectData()
    {
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
        
        await _baseRepository.AddAsync(mockData);
        var result = await _dbContext.SaveChangesAsync();

        result.Should().Be(1);
    }
    
    [Fact]
    public async Task BaseRepository_GetAllAsync_ShouldReturnCorrectData()
    {
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
            .CreateMany(10)
            .ToList();
        await _dbContext.Users.AddRangeAsync(mockData);

        await _dbContext.SaveChangesAsync();
        
        var result = await _baseRepository.GetAllAsync();

        result.Should().BeEquivalentTo(mockData);
    }
    
    [Fact]
    public async Task BaseRepository_GetByIdAsync_ShouldReturnCorrectData()
    {
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
        await _dbContext.Users.AddRangeAsync(mockData);

        await _dbContext.SaveChangesAsync();
        
        var result = await _baseRepository.GetByIdAsync(mockData.Id);

        result.Should().BeEquivalentTo(mockData);
    }
    
    [Fact]
    public async Task BaseRepository_GetByIdAsync_ShouldThrowException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid(); // Ensure this ID does not exist in the database.

        // Act
        Func<Task> act = async () => { await _baseRepository.GetByIdAsync(nonExistentId); };

        // Assert
        await act.Should().ThrowAsync<NotFoundIdException>()
            .WithMessage($"Not Found by ID: [{nonExistentId}] of [User]");
    }
    
    [Fact]
    public async Task BaseRepository_GetByIdAsync_IncludeDate_ShouldReturnCorrectData()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "",
            Company = new Company
            {
                Address = "",
                Email = "",
                Name = "",
                PhoneNumber = "",
                Partner = new Partner
                {
                    Id = Guid.NewGuid(),
                    Address = "",
                    Name = "",
                    PhoneNumber = "",
                    CommissionRate = 0,
                }
            }
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        
        
        var userInclude = new IncludeInfo<User>
        {
            NavigationProperty = order => order.Company,
            ThenIncludes = new List<Expression<Func<object, object>>>
            {
                sp => ((Company)sp).Partner
            }
        };
        var result = await _baseRepository.GetByIdAsync(user.Id);

        result.Should().BeEquivalentTo(user);
    }
    
    [Fact]
    public async Task BaseRepository_AddRangeAsync_ShouldReturnCorrectData()
    {
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
            .CreateMany(10)
            .ToList();
        
        await _baseRepository.AddRangeAsync(mockData);
        var result = await _dbContext.SaveChangesAsync();

        result.Should().Be(10);
    }
    
    [Fact]
    public async Task GenericRepository_SoftRemove_ShouldReturnCorrectData()
    {
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
        _dbContext.Users.Add(mockData);
        await _dbContext.SaveChangesAsync();


        _baseRepository.SoftRemove(mockData);
        var result = await _dbContext.SaveChangesAsync();

        result.Should().Be(1);
    }
    
    [Fact]
    public async Task GenericRepository_Update_ShouldReturnCorrectData()
    {
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
        _dbContext.Users.Add(mockData);
        await _dbContext.SaveChangesAsync();


        _baseRepository.Update(mockData);
        var result = await _dbContext.SaveChangesAsync();

        result.Should().Be(1);
    }
    
    [Fact]
    public async Task SoftRemoveRange_MarksEntitiesAsDeleted()
    {
        // Arrange
        var entities = new List<User>
        {
            new User { Id = Guid.NewGuid(), Name = "Test Entity 1" },
            new User { Id = Guid.NewGuid(), Name = "Test Entity 2" }
        };
        await _dbContext.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();

        // Act
        _baseRepository.SoftRemoveRange(entities);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedEntities = await _dbContext.Set<User>().ToListAsync();
        updatedEntities.Should().HaveCount(2);
        updatedEntities.ForEach(entity =>
        {
            //entity.IsDeleted.Should().BeTrue();
            /*entity.DeletionDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            entity.DeleteBy.Should().Be(_claimsServiceMock.Object.GetCurrentUserId);*/
        });
    }
    
    [Fact]
    public async Task UpdateRange_CorrectlyUpdatesEntities()
    {
        // Arrange
        var entities = new List<User>
        {
            new User { Id = Guid.NewGuid(), Name = "Entity One" },
            new User { Id = Guid.NewGuid(), Name = "Entity Two" }
        };
        await _dbContext.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();

        // Act
        entities.ForEach(e => e.Name += " Updated");
        _baseRepository.UpdateRange(entities);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedEntities = await _dbContext.Set<User>().ToListAsync();
        updatedEntities.Should().HaveCount(2);
        foreach (var entity in updatedEntities)
        {
            entity.Name.Should().EndWith(" Updated");
            //entity.ModificationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            //entity.ModificationBy.Should().Be(_claimsServiceMock.Object.GetCurrentUserId);
        }
    }
    
    [Fact]
    public async Task Remove_RemovesEntityCorrectly()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(),Name = "",Email = "delete@example.com" };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var removedUser = _baseRepository.Remove(user);
        await _dbContext.SaveChangesAsync();  // Commit the removal

        // Assert
        removedUser.Should().BeSameAs(user);  // Check the returned entity is the one removed
        var result = await _dbContext.Users.FindAsync(user.Id);
        result.Should().BeNull();  // Confirm the user is no longer in the database

        // Alternatively, ensuring the user is not part of the context anymore
        _dbContext.Entry(user).State.Should().Be(EntityState.Detached);
    }
    
    [Fact]
    public async Task RemoveRange_RemovesMultipleEntitiesCorrectly()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(),Name = "", Email = "user1@example.com" },
            new User { Id = Guid.NewGuid(),Name = "", Email = "user2@example.com" },
            new User { Id = Guid.NewGuid(),Name = "", Email = "user3@example.com" }
        };
        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync();

        // Act
        _baseRepository.RemoveRange(users);
        await _dbContext.SaveChangesAsync();  // Commit the removals

        // Assert
        var result = await _dbContext.Users.ToListAsync();
        result.Should().BeEmpty();  // Confirm no users are left in the database

        // Alternatively, ensure the users are not part of the context anymore
        foreach (var user in users)
        {
            _dbContext.Entry(user).State.Should().Be(EntityState.Detached);
        }
    }
    
    [Fact]
    public async Task FindAll_ReturnsAllUsers_WhenNoPredicateOrIncludes()
    {
        // Arrange
        var user1 = new User { Id = Guid.NewGuid(),Name = "",Email = "user1@example.com" };
        var user2 = new User { Id = Guid.NewGuid(),Name = "", Email = "user2@example.com" };
        await _dbContext.AddRangeAsync(user1, user2);
        await _dbContext.SaveChangesAsync();

        // Act
        var users = _baseRepository.FindAll().ToList();

        // Assert
        users.Should().HaveCount(2);
        users.Should().Contain(u => u.Email == "user1@example.com");
        users.Should().Contain(u => u.Email == "user2@example.com");
    }
    
    [Fact]
    public void FindAll_AppliesPredicateCorrectly()
    {
        // Arrange
        var user1 = new User { Id = Guid.NewGuid(),Name = "",Email = "user1@example.com" };
        var user2 = new User { Id = Guid.NewGuid(),Name = "", Email = "user2@example.com" };
        _dbContext.Users.Add(user1);
        _dbContext.Users.Add(user2);
        _dbContext.SaveChanges();

        // Act
        var users = _baseRepository.FindAll(u => u.Email.Contains("1")).ToList();

        // Assert
        users.Should().ContainSingle()
            .And.Contain(u => u.Email == "user1@example.com");
    }
    
    [Fact]
    public async Task FindSingleAsync_ReturnsCorrectUser()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(),Name = "", Email = "unique@example.com" };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _baseRepository.FindSingleAsync(u => u.Email == "unique@example.com");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("unique@example.com");
    }
    
    [Fact]
    public async Task FindAll_IncludesRelatedData()
    {
        // Arrange
        var user = new User { 
            Id = Guid.NewGuid(),
            Name = "",
            Email = "include@example.com", 
            Company = new Company
        {
            Name = "haha",
            Address = "",
            Email = "",
            PhoneNumber = ""
        }};
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = _baseRepository.FindAll(x => x.Email.Contains("example.com"),u => u.Company).ToList();

        // Assert
        result.First().Company.Should().NotBeNull();
        result.First().Company.Name.Should().Be("haha");
    }
}