using System.Linq.Expressions;
using FluentAssertions;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class UserRepositoryTests : SetupTest
{
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests()
    {
        _userRepository = new UserRepository(_dbContext);
    }
    
    [Fact]
    public async Task ToPagination_ReturnsPaginatedUsers()
    {
        // Arrange
        for (int i = 0; i < 15; i++) {
            _dbContext.Users.Add(new User { Id = Guid.NewGuid(),Name = $"user{i}", CreationDate = DateTime.UtcNow.AddDays(-i) });
        }
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userRepository.ToPagination(1, 10);

        // Assert
        result.Items.Should().HaveCount(5); // 15 - 10 = 5 on the second page
        result.TotalItemsCount.Should().Be(15);
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(10);
    }
    
    [Fact]
    public async Task ToPagination_ReturnsCorrectPaginationData()
    {
        // Arrange
        for (int i = 0; i < 25; i++) {
            _dbContext.Users.Add(new User { Id = Guid.NewGuid(),Name = "",CreationDate = DateTime.UtcNow.AddDays(-i) });
        }
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userRepository.ToPagination(1, 10); // second page, pageSize = 10

        // Assert
        result.Items.Should().HaveCount(10); // 10 items per page
        result.TotalItemsCount.Should().Be(25); // total 25 items added
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Items.First().CreationDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-10), TimeSpan.FromSeconds(1)); // Ordered by CreationDate
    }

    [Fact]
    public async Task ToPagination_AppliesPredicateCorrectly()
    {
        // Arrange
        for (int i = 0; i < 20; i++) {
            _dbContext.Users.Add(new User { Id = Guid.NewGuid(),Name = "", Email = i % 2 == 0 ? "even@example.com" : "odd@example.com" });
        }
        await _dbContext.SaveChangesAsync();
        Expression<Func<User, bool>> predicate = u => u.Email.Contains("even");

        // Act
        var result = await _userRepository.ToPagination(0, 10, predicate);

        // Assert
        result.Items.Should().AllSatisfy(user => user.Email.Should().Contain("even"));
        result.TotalItemsCount.Should().Be(10); // Half should be "even"
    }

    [Fact]
    public async Task ToPagination_IncludesPropertiesCorrectly()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role { Id = roleId, Name = "CUSTOMER" };
        var user = new User { Id = Guid.NewGuid(),Name = "", UserRoles = new List<UserRole> { new UserRole { RoleId = roleId, Role = role } } };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        var includes = new IncludeInfo<User>
        {
            NavigationProperty = u => u.UserRoles,
            ThenIncludes = new List<Expression<Func<object, object>>>
            {
                sp => ((UserRole)sp).Role
            }
        };

        // Act
        var result = await _userRepository.ToPagination(0, 10, null, includes);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items.First().UserRoles.Should().ContainSingle();
        result.Items.First().UserRoles.First().Role.Name.Should().Be("CUSTOMER");
    }
    
    [Fact]
    public async Task CalculateCompanyCode_ReturnsNextCode()
    {
        var companyId = Guid.NewGuid();
        _dbContext.Users.Add(new User { Id = Guid.NewGuid(),Name = string.Empty, CompanyId = companyId, Code = 5 });
        await _dbContext.SaveChangesAsync();

        // Act
        var nextCode = await _userRepository.CalculateCompanyCode(companyId);

        // Assert
        nextCode.Should().Be(6);
    }
    
    [Fact]
    public async Task GetUserByCompanyId_IncludesRoles()
    {
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var user = new User { Id = Guid.NewGuid(),Name = string.Empty, CompanyId = companyId, UserRoles = new List<UserRole> { new UserRole { RoleId = roleId, Role = new Role { Name = "CUSTOMER" } } } };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var users = await _userRepository.GetUserByCompanyId(companyId);

        // Assert
        users.Should().ContainSingle();
        users.First().UserRoles.Should().ContainSingle(ur => ur.Role.Name == "CUSTOMER");
    }
    
    [Fact]
    public async Task CalculateDeliveryCode_ReturnsNextCode()
    {
        var deliveryUnitId = Guid.NewGuid();
        _dbContext.Users.Add(new User { Id = Guid.NewGuid(),Name = string.Empty, DeliveryId = deliveryUnitId, Code = 101 });
        await _dbContext.SaveChangesAsync();

        var nextCode = await _userRepository.CalculateDeliveryCode(deliveryUnitId);

        nextCode.Should().Be(102);
    }
    
    [Fact]
    public async Task CalculatePartnerCode_ReturnsNextCode()
    {
        var managementUnitId = Guid.NewGuid();
        _dbContext.Users.Add(new User { Id = Guid.NewGuid(), Name = string.Empty,PartnerId = managementUnitId, Code = 202 });
        await _dbContext.SaveChangesAsync();

        var nextCode = await _userRepository.CalculatePartnerCode(managementUnitId);

        nextCode.Should().Be(203);
    }
    
    [Fact]
    public async Task CalculateSupplierCode_ReturnsNextCode()
    {
        var supplierId = Guid.NewGuid();
        _dbContext.Users.Add(new User { Id = Guid.NewGuid(),Name = string.Empty, SupplierId = supplierId, Code = 303 });
        await _dbContext.SaveChangesAsync();

        var nextCode = await _userRepository.CalculateSupplierCode(supplierId);

        nextCode.Should().Be(304);
    }
    
    [Fact]
    public async Task GetUserByIdAsync_ReturnUser()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = string.Empty};
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = await _userRepository.GetUserByIdAsync(userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
    }
    
    [Fact]
    public async Task GetUserByIdAsync_IncludesProperties_ReturnUser()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = string.Empty,
            Company = new Company
            {
                Email = "",
                Address = "",
                Name = "",
                PhoneNumber = "",
                Partner = new Partner
                {
                    Name = "Haha",
                    Address = "",
                    CommissionRate = 0,
                    PhoneNumber = ""
                }
            }
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        var userInclude = new IncludeInfo<User>
        {
            NavigationProperty = order => order.Company,
            ThenIncludes = new List<Expression<Func<object, object>>>
            {
                sp => ((Company)sp).Partner
            }
        };

        var result = await _userRepository.GetUserByIdAsync(userId,userInclude);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Company.Partner.Name.Should().Be("Haha");
    }
    
    [Fact]
    public async Task GetInfoCurrentUserById_ReturnsUserWithDetails()
    {
        var userId = Guid.NewGuid();
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Address = string.Empty
        };
        var user = new User { Id = userId,Name = "",Company = company };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = await _userRepository.GetInfoCurrentUserById(userId);

        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Company.Should().NotBeNull();
        result.Company.Id.Should().Be(company.Id);
    }
    
    [Fact]
    public async Task GetUserByEmail_ReturnsUser()
    {
        var email = "test@example.com";
        var user = new User { Id = Guid.NewGuid(),Name = string.Empty,Email = email };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = await _userRepository.GetUserByEmail(email);

        result.Should().NotBeNull();
        result.Email.Should().Be(email);
    }
    
    [Fact]
    public async Task GetUserBySupplierIds_ReturnsUsers()
    {
        var supplierId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            SupplierId = supplierId,
            Name = string.Empty,
            UserRoles = new List<UserRole> { new UserRole { RoleId = roleId, Role = new Role { Name = "SUPPLIER ADMIN" } } }
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = await _userRepository.GetUserBySupplierIds(new List<Guid> { supplierId });

        result.Should().ContainSingle();
        result.First().Id.Should().Be(user.Id);
    }
    
    [Fact]
    public async Task GetPartnerAdminByPartnerId_ReturnsPartnerAdmins()
    {
        var partnerId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            PartnerId = partnerId,
            Name = string.Empty,
            UserRoles = new List<UserRole> { new UserRole { RoleId = roleId, Role = new Role { Name = "PARTNER ADMIN" } } }
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = await _userRepository.GetPartnerAdminByPartnerId(partnerId);

        result.Should().ContainSingle();
        result.First().Id.Should().Be(user.Id);
    }
}