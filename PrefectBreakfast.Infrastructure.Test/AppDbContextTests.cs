using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test;

public class AppDbContextTests : SetupTest
{
    [Fact]
    public async Task AppDbContext_CategoriesDbSetShouldReturnCorrectData()
    {

        var mockData = _fixture.Build<Category>().Without(x => x.Foods).CreateMany(10).ToList();
        await _dbContext.Categories.AddRangeAsync(mockData);
            
        await _dbContext.SaveChangesAsync();

        var result = await _dbContext.Categories.ToListAsync();
        result.Should().BeEquivalentTo(mockData);
    }
    
    [Fact]
    public async Task AppDbContext_UsersDbSetShouldReturnEmptyListWhenNotHavingData()
    {
        var result = await _dbContext.Categories.ToListAsync();
        result.Should().BeEmpty();
    }
}