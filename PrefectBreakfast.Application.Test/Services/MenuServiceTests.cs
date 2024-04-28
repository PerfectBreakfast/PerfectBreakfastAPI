using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.MenuModels.Request;
using PerfectBreakfast.Application.Models.MenuModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class MenuServiceTests : SetupTest
{
    private readonly IMenuService _menuService;

    public MenuServiceTests()
    {
        _menuService = new MenuService(_unitOfWorkMock.Object, _mapper, _currentTimeMock.Object, _caheMock.Object);
    }
    
    private Menu GetTestMenu(Guid id = default)
    {
        return new Menu { Id = id == default ? Guid.NewGuid() : id, Name = "Test Menu" };
    }
    
    private Menu GetTestMenuWithMenuFood(Guid id = default)
    {
        return new Menu
        {
            Id = id == default ? Guid.NewGuid() : id, 
            Name = "Test Menu",
            MenuFoods = new List<MenuFood?>
            {
                new MenuFood
                {
                    ComboId = Guid.NewGuid(),
                },
                new MenuFood
                {
                    FoodId = Guid.NewGuid(),
                }
            }
        };
    }

    private MenuFood GetTestMenuFood(Guid menuId, Guid? foodId = null, Guid? comboId = null)
    {
        return new MenuFood { Id = Guid.NewGuid(), MenuId = menuId, FoodId = foodId, ComboId = comboId };
    }

    /*private ComboAndFoodResponse GetTestComboAndFoodResponse(Guid id, string name, bool isCombo = true)
    {
        if (isCombo)
        {
            return new ComboAndFoodResponse { Id = id, Name = name, Content = "Test Content" };
        }
        else
        {
            return new FoodResponse { Id = id, Name = name, Price = 10.0m };
        }
    }*/
    
    [Fact]
    public async Task CreateMenu_Success_MapsAndSaves()
    {
        // Arrange
        var request = new CreateMenuFoodRequest { Name = "Test Menu" };
        var menuFoodRequest1 = new MenuFoodRequest { FoodId = Guid.NewGuid() };
        var menuFoodRequest2 = new MenuFoodRequest { ComboId = Guid.NewGuid() };
        request.MenuFoodRequests = new List<MenuFoodRequest>() { menuFoodRequest1, menuFoodRequest2 };

        var menu = GetTestMenu();
        var menuFood1 = GetTestMenuFood(menu.Id, menuFoodRequest1.FoodId);
        var menuFood2 = GetTestMenuFood(menu.Id, comboId: menuFoodRequest2.ComboId);
        menu.MenuFoods = new List<MenuFood>() { menuFood1, menuFood2 };

        //_mapperMock.Setup(m => m.Map<Menu>(request)).Returns(menu);
        _unitOfWorkMock.Setup(u => u.MenuRepository.AddAsync(It.IsAny<Menu>())).ReturnsAsync(menu);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _menuService.CreateMenu(request);

        // Assert
        result.IsError.Should().BeFalse();
        //_mapperMock.Verify(m => m.Map<Menu>(request), Times.Once);
        //_unitOfWorkMock.Verify(u => u.MenuRepository.AddAsync(menu), Times.Once);
    }

    [Fact]
    public async Task CreateMenu_FoodNotFound_ReturnsError()
    {
        // Arrange
        var request = new CreateMenuFoodRequest { Name = "Test Menu" };
        var menuFoodRequest = new MenuFoodRequest { FoodId = Guid.NewGuid() };
        request.MenuFoodRequests = new List<MenuFoodRequest>() { menuFoodRequest };

        //_mapperMock.Setup(m => m.Map<Menu>(request)).Returns(GetTestMenu());
        _unitOfWorkMock.Setup(u => u.MenuRepository.AddAsync(It.IsAny<Menu>())).ReturnsAsync(GetTestMenu());
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(0); // Simulate failing save

        // Act
        var result = await _menuService.CreateMenu(request);

        // Assert
        result.IsError.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetMenuPaginationAsync_WithSearchTerm_ReturnsPagedData()
    {
        // Arrange
        const string searchTerm = "test";
        const int pageIndex = 1;
        const int pageSize = 10;
        var expectedTotalItems = 20;
        var expectedMenus = new List<Menu>() { GetTestMenu(), GetTestMenu() };

        _unitOfWorkMock.Setup(u => u.MenuRepository.ToPagination(pageIndex, pageSize, m => m.Name.ToLower().Contains(searchTerm.ToLower()) && !m.IsDeleted))
            .ReturnsAsync(new Pagination<Menu>
            {
                Items = expectedMenus,
                TotalItemsCount = expectedTotalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            });
        //_mapperMock.Setup(m => m.Map<List<MenuResponsePaging>>(expectedMenus)).Returns(expectedMenus.Select(m => new MenuResponsePaging { Id = m.Id, Name = m.Name }).ToList());

        // Act
        var result = await _menuService.GetMenuPaginationAsync(searchTerm, pageIndex, pageSize);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.PageIndex.Should().Be(pageIndex);
        result.Payload.PageSize.Should().Be(pageSize);
        result.Payload.TotalItemsCount.Should().Be(expectedTotalItems);
        //result.Payload.Items.Should().BeEquivalentTo(expectedMenus.Select(m => new MenuResponsePaging { Id = m.Id, Name = m.Name }));
    }
    
    [Fact]
    public async Task GetMenuPaginationAsync_NoSearchTerm_ReturnsPagedData()
    {
        // Arrange
        const int pageIndex = 1;
        const int pageSize = 10;
        var expectedTotalItems = 20;
        var expectedMenus = new List<Menu>() { GetTestMenu(), GetTestMenu() };

        _unitOfWorkMock.Setup(u => u.MenuRepository.ToPagination(pageIndex, pageSize, m => !m.IsDeleted))
            .ReturnsAsync(new Pagination<Menu>
            {
                Items = expectedMenus,
                TotalItemsCount = expectedTotalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            });
        //_mapperMock.Setup(m => m.Map<List<MenuResponsePaging>>(expectedMenus)).Returns(expectedMenus.Select(m => new MenuResponsePaging { Id = m.Id, Name = m.Name }).ToList());

        // Act
        var result = await _menuService.GetMenuPaginationAsync(null, pageIndex, pageSize);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.PageIndex.Should().Be(pageIndex);
        result.Payload.PageSize.Should().Be(pageSize);
        result.Payload.TotalItemsCount.Should().Be(expectedTotalItems);
        //result.Payload.Items.Should().BeEquivalentTo(expectedMenus.Select(m => new MenuResponsePaging { Id = m.Id, Name = m.Name }));
    }
    
    [Fact]
    public async Task GetMenus_Success_ReturnsMappedMenus()
    {
        // Arrange
        var expectedMenus = new List<Menu>() { GetTestMenuWithMenuFood(), GetTestMenuWithMenuFood() };
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetAllAsync()).ReturnsAsync(expectedMenus);
        //_mapperMock.Setup(m => m.Map<List<MenuResponse>>(expectedMenus)).Returns(expectedMenus.Select(m => new MenuResponse { Id = m.Id, Name = m.Name }).ToList());
        var menuResponse = _mapper.Map<List<MenuResponse>>(expectedMenus);
        // Act
        var result = await _menuService.GetMenus();

        // Assert
        result.IsError.Should().BeFalse();
        //result.Payload.Should().BeEquivalentTo(expectedMenus.Select(m => new MenuResponse { Id = m.Id, Name = m.Name }));
    }
    
    [Fact]
    public async Task GetMenus_Exception_ReturnsUnknownError()
    {
        // Arrange
        var exceptionMessage = "Test Exception";
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetAllAsync()).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _menuService.GetMenus();

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == exceptionMessage);
    }
    
    [Fact]
    public async Task GetMenu_Success_ReturnsMappedMenu()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var expectedMenu = GetTestMenuWithMenuFood(menuId);
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetMenuFoodByIdAsync(menuId)).ReturnsAsync(expectedMenu);
        //_mapperMock.Setup(m => m.Map<MenuResponse>(expectedMenu)).Returns(new MenuResponse { Id = expectedMenu.Id, Name = expectedMenu.Name });

        // Act
        var result = await _menuService.GetMenu(menuId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Payload.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetMenu_NotFound_ReturnsUnknownError()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetMenuFoodByIdAsync(menuId)).ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _menuService.GetMenu(menuId);

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task GetMenu_Exception_ReturnsUnknownError()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var exceptionMessage = "Test Exception";
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetMenuFoodByIdAsync(menuId)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _menuService.GetMenu(menuId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == exceptionMessage);
    }
    
    [Fact]
    public async Task DeleteMenu_Success_RemovesMenuAndSaves()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var menu = GetTestMenu(menuId);
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(menuId)).ReturnsAsync(menu);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _menuService.DeleteMenu(menuId);

        // Assert
        result.IsError.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.MenuRepository.SoftRemove(menu), Times.Once);
    }
    
    [Fact]
    public async Task DeleteMenu_SelectedMenu_ReturnsBadRequest()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var menu = GetTestMenu(menuId);
        menu.IsSelected = true;
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(menuId)).ReturnsAsync(menu);

        // Act
        var result = await _menuService.DeleteMenu(menuId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.BadRequest && e.Message == "Menu is currently selected");
    }
    
    [Fact]
    public async Task DeleteMenu_NotFound_ReturnsUnknownError()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(menuId)).ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _menuService.DeleteMenu(menuId);

        // Assert
        result.IsError.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteMenu_Exception_ReturnsUnknownError()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var exceptionMessage = "Test Exception";
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(menuId)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _menuService.DeleteMenu(menuId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == exceptionMessage);
    }
    
    [Fact]
    public async Task ChooseMenu_Success_DeselectsExistingAndSelectsNew_Saves()
    {
        // Arrange
        var existingMenuId = Guid.NewGuid();
        var newMenuId = Guid.NewGuid();
        var existingMenu = GetTestMenu(existingMenuId);
        existingMenu.IsSelected = true;
        var newMenu = GetTestMenu(newMenuId);
        newMenu.IsSelected = false;

        _unitOfWorkMock.Setup(u => u.MenuRepository.FindSingleAsync(m => m.IsSelected == true)).ReturnsAsync(existingMenu);
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(newMenuId)).ReturnsAsync(newMenu);

        // Act
        var result = await _menuService.ChooseMenu(newMenuId);

        // Assert
        result.IsError.Should().BeFalse();

        // Verify deselect
        existingMenu.IsSelected.Should().BeFalse();

        // Verify select
        newMenu.IsSelected.Should().BeTrue();

        // Verify updates were saved
        _unitOfWorkMock.Verify(u => u.MenuRepository.Update(existingMenu), Times.Once);
        _unitOfWorkMock.Verify(u => u.MenuRepository.Update(newMenu), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task ChooseMenu_NoExistingSelected_SelectsNew_Saves()
    {
        // Arrange
        var newMenuId = Guid.NewGuid();
        var newMenu = GetTestMenu(newMenuId);

        _unitOfWorkMock.Setup(u => u.MenuRepository.FindSingleAsync(m => m.IsSelected == true)).ReturnsAsync((Menu?)null);
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(newMenuId)).ReturnsAsync(newMenu);

        // Act
        var result = await _menuService.ChooseMenu(newMenuId);

        // Assert
        result.IsError.Should().BeFalse();

        // Verify select
        newMenu.IsSelected.Should().BeTrue();

        // Verify update was saved
        _unitOfWorkMock.Verify(u => u.MenuRepository.Update(newMenu), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task ChooseMenu_NotFound_ReturnsUnknownError()
    {
        // Arrange
        var newMenuId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.MenuRepository.FindSingleAsync(m => m.IsSelected == true)).ReturnsAsync((Menu?)null);
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(newMenuId)).ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _menuService.ChooseMenu(newMenuId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == "Id is not exsit");
    }
    
    [Fact]
    public async Task ChooseMenu_Exception_ReturnsUnknownError()
    {
        // Arrange
        var newMenuId = Guid.NewGuid();
        var exceptionMessage = "Test Exception";

        _unitOfWorkMock.Setup(u => u.MenuRepository.FindSingleAsync(m => m.IsSelected == true)).ReturnsAsync((Menu?)null);
        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(newMenuId)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _menuService.ChooseMenu(newMenuId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == exceptionMessage);
    }
    
    [Fact]
    public async Task UpdateMenu_Success_RemovesExistingAndAddsNewMenuFoods()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var existingMenu = GetTestMenuWithMenuFood(menuId/*, includeMenuFoods: true*/);
        var existingMenuFoodCount = existingMenu.MenuFoods.Count;
        var updateRequest = new UpdateMenuRequest
        {
            Name = "Updated Menu",
            MenuFoodRequests = new List<MenuFoodRequest>
            {
                new MenuFoodRequest { FoodId = Guid.NewGuid() },
            }
        };

        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(menuId, m => m.MenuFoods))
            .ReturnsAsync(existingMenu);
        _unitOfWorkMock.Setup(u => u.MenuFoodRepository.RemoveRange(It.IsAny<List<MenuFood>>())).Callback<List<MenuFood>>(removedFoods => removedFoods.Count.Should().Be(existingMenuFoodCount));
        _unitOfWorkMock.Setup(u => u.MenuRepository.Update(It.IsAny<Menu>()));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _menuService.UpdateMenu(menuId, updateRequest);

        // Assert
        result.IsError.Should().BeFalse();
        _unitOfWorkMock.VerifyAll(); // Verify all mocks were called
    }
    
    [Fact]
    public async Task UpdateMenu_EmptyMenuFoodRequests_RemovesExisting()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var existingMenu = GetTestMenuWithMenuFood(menuId);
        var existingMenuFoodCount = existingMenu.MenuFoods.Count;
        var updateRequest = new UpdateMenuRequest
        {
            Name = "Updated Menu",
            MenuFoodRequests = new List<MenuFoodRequest>() // Empty list
        };

        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(menuId, m => m.MenuFoods))
            .ReturnsAsync(existingMenu);
        _unitOfWorkMock.Setup(u => u.MenuFoodRepository.RemoveRange(It.IsAny<List<MenuFood>>())).Callback<List<MenuFood>>(removedFoods => removedFoods.Count.Should().Be(existingMenuFoodCount));
        _unitOfWorkMock.Setup(u => u.MenuRepository.Update(It.IsAny<Menu>()));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _menuService.UpdateMenu(menuId, updateRequest);

        // Assert
        result.IsError.Should().BeFalse();
        _unitOfWorkMock.VerifyAll(); // Verify all mocks were called
    }
    
    [Fact]
    public async Task UpdateMenu_NotFound_ReturnsNotFound()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var updateRequest = new UpdateMenuRequest { Name = "Updated Menu" };

        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(menuId, m => m.MenuFoods)).ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _menuService.UpdateMenu(menuId, updateRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound && e.Message == "Id is not exits");
    }
    
    [Fact]
    public async Task UpdateMenu_Exception_ReturnsUnknownError()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var updateRequest = new UpdateMenuRequest { Name = "Updated Menu" };
        var exceptionMessage = "Test Exception";

        _unitOfWorkMock.Setup(u => u.MenuRepository.GetByIdAsync(menuId, m => m.MenuFoods)).ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _menuService.UpdateMenu(menuId, updateRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.UnknownError && e.Message == exceptionMessage);
    }
}