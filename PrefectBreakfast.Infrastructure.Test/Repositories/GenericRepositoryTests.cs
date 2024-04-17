using AutoFixture;
using FluentAssertions;
using Google.Apis.Util;
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
}