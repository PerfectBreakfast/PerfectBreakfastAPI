using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class GenericRepositoryTests : SetupTest
{
    private readonly IGenericRepository<Category> _genericRepository;
    
    public GenericRepositoryTests()
    {
        _genericRepository = new GenericRepository<Category>(
            _dbContext,
            _currentTimeMock.Object,
            _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GenericRepository_GetAllAsync_ShouldReturnCorrectData()
    {
        var mockData = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .CreateMany(10)
            .ToList();
        await _dbContext.Categories.AddRangeAsync(mockData);

        await _dbContext.SaveChangesAsync();
        
        var result = await _genericRepository.GetAllAsync();

        result.Should().BeEquivalentTo(mockData);
    }
    
    [Fact]
    public async Task GenericRepository_GetAllAsync_ShouldReturnEmptyWhenHaveNoData()
    {

        var result = await _genericRepository.GetAllAsync();

        result.Should().BeEmpty();
    }
    
    
    [Fact]
    public async Task GenericRepository_GetByIdAsync_ShouldReturnCorrectData()
    {
        var mockData = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .Create();
        await _dbContext.Categories.AddRangeAsync(mockData);

        await _dbContext.SaveChangesAsync();
        
        var result = await _genericRepository.GetByIdAsync(mockData.Id);

        result.Should().BeEquivalentTo(mockData);
    }
    
    [Fact]
    public async Task GetByIdAndIncludeAsync_IncludesSpecifiedRelations()
    {
        var entity = new Category
        {
            Name = "Include Test",
            Foods = new List<Food?>
            {
                new Food
                {
                    Id = Guid.NewGuid(),
                    Name = "",
                    ComboFoods = new List<ComboFood?>
                    {
                        new ComboFood
                        {
                            ComboId = Guid.NewGuid(),
                            FoodId = Guid.NewGuid()
                        }
                    }
                }
            }
        };
        _dbContext.Set<Category>().Add(entity);
        await _dbContext.SaveChangesAsync();

        // Assuming TestEntity has a navigation property 'Details' to include
        var categoryInclude = new IncludeInfo<Category>
        {
            NavigationProperty = x => x.Foods,
            ThenIncludes = new List<Expression<Func<object, object>>>
            {
                sp => ((Food)sp).ComboFoods
            }
        };
        var fetchedEntity = await _genericRepository.GetByIdAndIncludeAsync(entity.Id, categoryInclude);

        fetchedEntity.Foods.Should().NotBeNull();
        fetchedEntity.Foods.First().Should().NotBeNull();
    }
    
    [Fact]
    public async Task GenericRepository_AddAsync_ShouldReturnCorrectData()
    {
        var mockData = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .Create();
        
        await _genericRepository.AddAsync(mockData);
        var result = await _dbContext.SaveChangesAsync();

        result.Should().Be(1);
    }
    
    [Fact]
    public async Task GenericRepository_AddRangeAsync_ShouldReturnCorrectData()
    {
        var mockData = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .CreateMany(10).ToList();
        
        await _genericRepository.AddRangeAsync(mockData);
        var result = await _dbContext.SaveChangesAsync();

        result.Should().Be(10);
    }
    
    [Fact]
    public async Task GenericRepository_SoftRemove_ShouldReturnCorrectData()
    {
        var mockData = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .Create();
        _dbContext.Categories.Add(mockData);
        await _dbContext.SaveChangesAsync();


        _genericRepository.SoftRemove(mockData);
        var result = await _dbContext.SaveChangesAsync();

        result.Should().Be(1);
    }
    
    [Fact]
    public async Task GenericRepository_Update_ShouldReturnCorrectData()
    {
        var mockData = _fixture.Build<Category>()
            .Without(x => x.Foods)
            .Create();
        _dbContext.Categories.Add(mockData);
        await _dbContext.SaveChangesAsync();


        _genericRepository.Update(mockData);
        var result = await _dbContext.SaveChangesAsync();

        result.Should().Be(1);
    }
    
    [Fact]
    public async Task ToPagination_ReturnsPaginatedResult()
    {
        for (int i = 0; i < 30; i++) {
            _dbContext.Set<Category>().Add(new Category { Name = $"Entity {i}" });
        }
        await _dbContext.SaveChangesAsync();

        var result = await _genericRepository.ToPagination(2, 10);

        result.PageIndex.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalItemsCount.Should().Be(30);
        result.Items.Should().HaveCount(10);
    }
    
    [Fact]
    public async Task ToPagination_AppliesPredicateAndIncludesCorrectly()
    {
        // Arrange
        for (int i = 0; i < 50; i++)
        {
            _dbContext.Add(new Category { Id = Guid.NewGuid(), CreationDate = DateTime.UtcNow.AddDays(-i), Name = i % 2 == 0 ? "Even" : "Odd" });
        }
        await _dbContext.SaveChangesAsync();

        Expression<Func<Category, bool>> predicate = e => e.Name == "Even";
        var includeProps = new IncludeInfo<Category>[]
        {
            new IncludeInfo<Category>
            {
                NavigationProperty = e => e.Foods
            }
        };

        // Act
        var result = await _genericRepository.ToPagination(0, 10, predicate, includeProps);

        // Assert
        result.Items.Should().AllSatisfy(e => e.Name.Should().Be("Even"));
        result.Items.Should().HaveCount(10);
        result.TotalItemsCount.Should().Be(25); // Half of 50 are "Even"
    }
    
    [Fact]
    public async Task UpdateRange_CorrectlyUpdatesEntities()
    {
        // Arrange
        var entities = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Entity One" },
            new Category { Id = Guid.NewGuid(), Name = "Entity Two" }
        };
        await _dbContext.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();

        // Act
        entities.ForEach(e => e.Name += " Updated");
        _genericRepository.UpdateRange(entities);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedEntities = await _dbContext.Set<Category>().ToListAsync();
        updatedEntities.Should().HaveCount(2);
        foreach (var entity in updatedEntities)
        {
            entity.Name.Should().EndWith(" Updated");
            //entity.ModificationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            //entity.ModificationBy.Should().Be(_claimsServiceMock.Object.GetCurrentUserId);
        }
    }
    
    [Fact]
    public async Task SoftRemoveRange_MarksEntitiesAsDeleted()
    {
        // Arrange
        var entities = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Test Entity 1" },
            new Category { Id = Guid.NewGuid(), Name = "Test Entity 2" }
        };
        await _dbContext.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();

        // Act
        _genericRepository.SoftRemoveRange(entities);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedEntities = await _dbContext.Set<Category>().ToListAsync();
        updatedEntities.Should().HaveCount(2);
        updatedEntities.ForEach(entity =>
        {
            entity.IsDeleted.Should().BeTrue();
            /*entity.DeletionDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            entity.DeleteBy.Should().Be(_claimsServiceMock.Object.GetCurrentUserId);*/
        });
    }
}