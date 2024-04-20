using System.Linq.Expressions;
using FluentAssertions;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class SupplierRepositoryTests : SetupTest
{
    private readonly SupplierRepository _supplierRepository;

    public SupplierRepositoryTests()
    {
        _supplierRepository = new SupplierRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetSupplierDetail_ReturnsSupplierWithDetails()
    {
        var supplierId = Guid.NewGuid();
        var partner = new Partner
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty,
            CommissionRate = 0
        };
        var food = new Food { Id = Guid.NewGuid(), IsDeleted = false };
        var supplier = new Supplier
        {
            Id = supplierId,
            SupplyAssignments = new List<SupplyAssignment>
            {
                new SupplyAssignment
                {
                    Partner = partner
                }
            },
            SupplierCommissionRates = new List<SupplierCommissionRate>
            {
                new SupplierCommissionRate
                {
                    Food = food
                }
            },
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty
        };

        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync();

        var result = await _supplierRepository.GetSupplierDetail(supplierId);

        result.Should().NotBeNull();
        result.SupplyAssignments.Should().ContainSingle();
        result.SupplierCommissionRates.Should().ContainSingle();
    }
    
    [Fact]
    public async Task GetSupplierByPartner_ReturnsSuppliers()
    {
        var partnerId = Guid.NewGuid();
        var supplier = new Supplier
        {
            SupplyAssignments = new List<SupplyAssignment>
            {
                new SupplyAssignment
                {
                    PartnerId = partnerId
                }
            },
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty
        };

        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync();

        var results = await _supplierRepository.GetSupplierByPartner(partnerId);

        results.Should().ContainSingle();
    }
    
    [Fact]
    public async Task GetSupplierById_UsesDynamicIncludes()
    {
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier
        {
            Id = supplierId,
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty
        };
        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync();

        Expression<Func<Supplier, object>> include = s => s.SupplyAssignments;

        var result = await _supplierRepository.GetSupplierById(supplierId, include);

        result.Should().NotBeNull();
        result.Id.Should().Be(supplierId);
    }
    
    [Fact]
    public async Task GetSupplierFoodAssignmentByPartner_IncludesRelatedData()
    {
        var partnerId = Guid.NewGuid();
        var supplier = new Supplier
        {
            SupplyAssignments = new List<SupplyAssignment>
            {
                new SupplyAssignment
                {
                    PartnerId = partnerId
                }
            },
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty
        };

        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync();

        var results = await _supplierRepository.GetSupplierFoodAssignmentByPartner(partnerId);

        results.Should().ContainSingle();
    }
    
    [Fact]
    public async Task GetSupplierFoodAssignmentBySupplier_ReturnsSupplierWithNestedData()
    {
        var supplierId = Guid.NewGuid();
        var supplier = new Supplier
        {
            Id = supplierId,
            Name = string.Empty,
            Address = string.Empty,
            PhoneNumber = string.Empty,
            SupplierCommissionRates = new List<SupplierCommissionRate>
            {
                new SupplierCommissionRate
                {
                    SupplierFoodAssignments = new List<SupplierFoodAssignment>
                    {
                        new SupplierFoodAssignment
                        {
                            Food = new Food
                            {
                                Name = "Apple"
                            }
                        }
                    }
                }
            },
            
        };

        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync();

        var result = await _supplierRepository.GetSupplierFoodAssignmentBySupplier(supplierId);

        result.Should().NotBeNull();
        result.SupplierCommissionRates.First().SupplierFoodAssignments.Should().ContainSingle();
    }
}