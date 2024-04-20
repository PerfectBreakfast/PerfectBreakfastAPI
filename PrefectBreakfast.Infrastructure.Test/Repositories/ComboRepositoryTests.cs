using AutoFixture;
using FluentAssertions;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class ComboRepositoryTests : SetupTest
{
    private readonly IComboRepository _comboRepository;
    
    public ComboRepositoryTests()
    {
        _comboRepository = new ComboRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetAllComboNotDeleted_ShouldReturnOnlyActiveCombos()
    {
        // Arrange
        var combos = _fixture.Build<Combo>()
            .Without(x => x.ComboFoods)
            .Without(x => x.MenuFoods)
            .Without(x => x.OrderDetails)
            .With(c => c.IsDeleted, false)
            .CreateMany(5)
            .ToList();
        combos.AddRange(_fixture.Build<Combo>()
            .Without(x => x.ComboFoods)
            .Without(x => x.MenuFoods)
            .Without(x => x.OrderDetails)
            .With(c => c.IsDeleted, true)
            .CreateMany(5)
            .ToList());

        await _dbContext.Combos.AddRangeAsync(combos);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _comboRepository.GetAllComboNotDeleted();

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(c => !c.IsDeleted);
    }

    [Fact]
    public async Task GetComboFoodByIdAsync_ShouldReturnComboWithDetails()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var combo = new Combo
        {
            Id = comboId,
            Name = "",
            Content = "",
            IsDeleted = false,
            ComboFoods = new List<ComboFood>
            {
                new ComboFood { Food = new Food { Category = new Category() } }
            }
        };
        await _dbContext.Combos.AddAsync(combo);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _comboRepository.GetComboFoodByIdAsync(comboId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(comboId);
        result.ComboFoods.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllCombosWithPaginationAsync_ShouldPaginateCorrectly()
    {
        // Arrange
        var combos = Enumerable.Range(1, 50).Select(i => new ComboResponse()).ToList();
        var pageIndex = 1;
        var pageSize = 10;

        // Act
        var result = await _comboRepository.GetAllCombosWithPaginationAsync(pageIndex, pageSize, combos);

        // Assert
        result.PageIndex.Should().Be(pageIndex);
        result.PageSize.Should().Be(pageSize);
        result.TotalItemsCount.Should().Be(50);
        result.Items.Should().HaveCount(10);
    }
}