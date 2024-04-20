using FluentAssertions;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using PerfectBreakfast.Infrastructure.Repositories;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Infrastructure.Test.Repositories;

public class MenuRepositoryTests : SetupTest
{
    private readonly IMenuRepository _menuRepository;

    public MenuRepositoryTests()
    {
        _menuRepository = new MenuRepository(_dbContext, _currentTimeMock.Object, _claimServiceMock.Object);
    }
    
    [Fact]
    public async Task GetMenuFoodByIdAsync_ReturnsMenuWithDetails()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var food = new Food { Name = "Pizza", Description = "Delicious pizza", Price = 9.99M, Image = "pizza.jpg", FoodStatus = FoodStatus.Combo };
        var combo = new Combo { Name = "Lunch Combo", Content = "Includes drink and dessert", Image = "combo.jpg" };
        var menuFood = new MenuFood { Food = food, Combo = combo };
        var menu = new Menu
        {
            Id = menuId,
            Name = "Lunch Specials",
            IsSelected = false,
            MenuFoods = new List<MenuFood> { menuFood }
        };

        _dbContext.Menus.Add(menu);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _menuRepository.GetMenuFoodByIdAsync(menuId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(menuId);
        result.MenuFoods.Should().HaveCount(1);
        result.MenuFoods.First().Food.Name.Should().Be("Pizza");
        result.MenuFoods.First().Combo.Name.Should().Be("Lunch Combo");
    }
    
    [Fact]
    public async Task GetMenuFoodByStatusAsync_ReturnsSelectedMenuWithDetails()
    {
        // Arrange
        var selectedMenu = new Menu
        {
            Name = "",
            IsSelected = true,
            MenuFoods = new List<MenuFood>
            {
                new MenuFood
                {
                    Food = new Food
                    {
                        Name = "Salad",
                        Description = "Fresh Salad",
                        Price = 5.00M,
                        FoodStatus = FoodStatus.Combo
                    },
                    Combo = new Combo
                    {
                        Name = "Dinner Combo",
                        Content = "Dinner with dessert",
                        ComboFoods = new List<ComboFood>
                        {
                            new ComboFood
                            {
                                Food = new Food
                                {
                                    Name = "Cake",
                                    Price = 2.50M,
                                    FoodStatus = FoodStatus.Retail
                                }
                            }
                        }
                    }
                }
            },
        };
        var unselectedMenu = new Menu
        {
            IsSelected = false,
            Name = ""
        };
        _dbContext.Menus.AddRange(selectedMenu, unselectedMenu);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _menuRepository.GetMenuFoodByStatusAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSelected.Should().BeTrue();
        result.MenuFoods.Should().NotBeEmpty();
        result.MenuFoods.First().Food.Name.Should().Be("Salad");
        result.MenuFoods.First().Combo.Name.Should().Be("Dinner Combo");
        result.MenuFoods.First().Combo.ComboFoods.First().Food.Name.Should().Be("Cake");
    }
}