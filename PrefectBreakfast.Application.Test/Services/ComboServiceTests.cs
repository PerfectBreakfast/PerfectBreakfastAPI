using System.Linq.Expressions;
using FluentAssertions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ComboModels.Request;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class ComboServiceTests : SetupTest
{
    private readonly IComboService _comboService;

    public ComboServiceTests()
    {
        _comboService = new ComboService(_unitOfWorkMock.Object, _mapper, _imgurServiceMock.Object,
            _currentTimeMock.Object);
    }

    [Fact]
    public async Task GetCombo_ReturnsComboDetail_WhenComboExists()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var combo = new Combo
        {
            Id = comboId,
            ComboFoods = new List<ComboFood>
            {
                new ComboFood
                {
                    ComboId = comboId,
                    Food = new Food
                    {
                        Id = Guid.NewGuid(),
                        Name = "Apple",
                        Price = 1.25M,
                        Category = new Category
                        {
                            Id = Guid.NewGuid(),
                            Name = "hhaa"
                        }
                    }
                }
            }
        };
        var expectedResponse = _mapper.Map<ComboDetailResponse>(combo);
        expectedResponse.FoodResponses =
            _mapper.Map<List<FoodResponseCategory?>>(combo.ComboFoods.Select(cf => cf.Food).ToList());

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetComboFoodByIdAsync(comboId))
            .ReturnsAsync(combo);

        // Act
        var result = await _comboService.GetCombo(comboId);

        // Assert
        //result.IsSuccess.Should().BeTrue();
        result.Payload.Should().BeEquivalentTo(expectedResponse);
    }


    [Fact]
    public async Task GetCombo_ReturnsError_WhenComboDoesNotExist()
    {
        // Arrange
        var comboId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetComboFoodByIdAsync(comboId))
            .ReturnsAsync((Combo)null);

        // Act
        var result = await _comboService.GetCombo(comboId);

        // Assert
        //Assert.False(result.IsSuccess);
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.NotFound);
    }

    [Fact]
    public async Task GetCombo_ReturnsError_WhenThrowEx()
    {
        // Arrange
        var comboId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetComboFoodByIdAsync(comboId))
            .ThrowsAsync(new Exception(""));

        // Act
        var result = await _comboService.GetCombo(comboId);

        // Assert
        Assert.True(result.IsError);
        Assert.Null(result.Payload);
    }

    [Fact]
    public async Task DeleteCombo_Succeeds_WhenComboExistsAndNotInMenu()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var combo = new Combo
        {
            Id = comboId,
            Name = "",
            Content = ""
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(comboId))
            .ReturnsAsync(combo);

        _unitOfWorkMock.Setup(u => u.MenuFoodRepository.IsComboExistInMenu(comboId))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.ComboRepository.SoftRemove(combo));
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

        // Act
        var result = await _comboService.DeleteCombo(comboId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteCombo_ReturnsError_WhenComboInMenu()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var combo = new Combo
        {
            Id = comboId,
            Name = "",
            Content = ""
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(comboId))
            .ReturnsAsync(combo);

        _unitOfWorkMock.Setup(u => u.MenuFoodRepository.IsComboExistInMenu(comboId))
            .ReturnsAsync(true);

        // Act
        var result = await _comboService.DeleteCombo(comboId);

        // Assert
        //Assert.False(result.IsSuccess);
        result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCombo_HandlesNotFoundIdExceptionsProperly()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var combo = new Combo
        {
            Id = comboId,
            Name = "",
            Content = ""
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(comboId))
            .ThrowsAsync(new NotFoundIdException(""));

        _unitOfWorkMock.Setup(u => u.MenuFoodRepository.IsComboExistInMenu(comboId))
            .ReturnsAsync(true);

        // Act
        var result = await _comboService.DeleteCombo(comboId);

        // Assert
        Assert.Null(result.Payload);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task DeleteCombo_HandlesExceptionsProperly()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var combo = new Combo
        {
            Id = comboId,
            Name = "",
            Content = ""
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(comboId))
            .ThrowsAsync(new Exception(""));

        _unitOfWorkMock.Setup(u => u.MenuFoodRepository.IsComboExistInMenu(comboId))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ThrowsAsync(new Exception()); // Simulate failure to save changes

        // Act
        var result = await _comboService.DeleteCombo(comboId);

        // Assert
        Assert.Null(result.Payload);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task DeleteCombo_HandlesExceptionsProperlySaveChangeFail()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var combo = new Combo
        {
            Id = comboId,
            Name = "",
            Content = ""
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(comboId))
            .ReturnsAsync(combo);

        _unitOfWorkMock.Setup(u => u.MenuFoodRepository.IsComboExistInMenu(comboId))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ThrowsAsync(new Exception()); // Simulate failure to save changes

        // Act
        var result = await _comboService.DeleteCombo(comboId);

        // Assert
        Assert.Null(result.Payload);
        Assert.Single(result.Errors);
    }


    [Fact]
    public async Task GetCombos_ReturnsError_WhenExceptionIsThrown()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.ComboRepository.GetAllComboNotDeleted())
            .ThrowsAsync(new Exception());

        // Act
        var result = await _comboService.GetCombos();

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetCombos_ReturnsComboList_WhenCombosExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var combos = new List<Combo>()
        {
            new Combo
            {
                Id = id,
                Name = "Test Combo 1",
                Content = "This is a test combo",
                ComboFoods = new List<ComboFood>()
                {
                    new ComboFood
                    {
                        Food = new Food
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Food 1",
                            Price = 10.50M,
                        },
                        ComboId = id
                    }
                }
            },
            // Add more test combos with different structures
        };

        var expectedResponse = _mapper.Map<List<ComboResponse>>(combos);

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetAllComboNotDeleted())
            .ReturnsAsync(combos);

        // Act
        var result = await _comboService.GetCombos();

        // Assert
        //result.IsSuccess.Should().BeTrue();
        result.Payload.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task CreateCombo_ReturnsSuccess_WithValidData()
    {
        // Arrange
        var comboId = Guid.NewGuid();
        var createRequest = new CreateComboRequest
        {
            Name = "Test Combo",
            Content = "This is a test combo",
            FoodId = new List<Guid?>() { Guid.NewGuid(), Guid.NewGuid() }
        };

        var combo = _mapper.Map<Combo>(createRequest);
        combo.Id = comboId;
        combo.ComboFoods = createRequest.FoodId.Select(foodId => new ComboFood { FoodId = foodId }).ToList();
        combo.Image = "imgur_image_link"; // Assuming ImgurService returns a URL

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetAllComboNotDeleted())
            .ReturnsAsync(new List<Combo>()); // No existing combos

        /*_mapper.Setup(m => m.Map<Combo>(It.IsAny<CreateComboRequest>()))
            .Returns(combo);*/

        _imgurServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(combo.Image);

        _unitOfWorkMock.Setup(u => u.ComboRepository.AddAsync(It.IsAny<Combo>()))
            .Verifiable();

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _comboService.CreateCombo(createRequest);

        // Assert
        _unitOfWorkMock.Verify(u => u.ComboRepository.AddAsync(It.IsAny<Combo>()), Times.Once);
    }


    [Fact]
    public async Task CreateCombo_ReturnsError_WhenComboAlreadyExists()
    {
        // Arrange
        var existingCombo = new Combo
        {
            Id = Guid.NewGuid(),
            Name = "Existing Combo",
            Content = "This is an existing combo",
            ComboFoods = new List<ComboFood>()
        };
        var createRequest = new CreateComboRequest
        {
            Name = "Test Combo",
            Content = "This is a test combo",
            FoodId = existingCombo.ComboFoods.Select(cf => cf.FoodId).ToList()
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetAllComboNotDeleted())
            .ReturnsAsync(new List<Combo>() { existingCombo });

        // Act
        var result = await _comboService.CreateCombo(createRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();

        result.Errors.Should().ContainSingle(e =>
            e.Code == ErrorCode.BadRequest &&
            e.Message.Contains(existingCombo.Name));

        // No calls to ImgurService or repository methods are expected here
        _imgurServiceMock.Verify(s => s.UploadImageAsync(It.IsAny<IFormFile>()), Times.Never);
    }

    [Fact]
    public async Task CreateCombo_ReturnsError_WhenSavingChangesFails()
    {
        // Arrange
        var createRequest = new CreateComboRequest
        {
            Name = "Test Combo",
            Content = "This is a test combo",
            FoodId = new List<Guid?>() { Guid.NewGuid(), Guid.NewGuid() }
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetAllComboNotDeleted())
            .ReturnsAsync(new List<Combo>()); // No existing combos

        /*_mapper.Setup(m => m.Map<Combo>(It.IsAny<CreateComboRequest>()))
            .Returns(new Combo()); // Don't need specific details for this test*/

        _imgurServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("imgur_image_link"); // Assuming successful upload

        _unitOfWorkMock.Setup(u => u.ComboRepository.AddAsync(It.IsAny<Combo>()))
            .Verifiable(); // Expect AddAsync to be called

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ThrowsAsync(new Exception("Saving changes failed")); // Simulate save error

        // Act
        var result = await _comboService.CreateCombo(createRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();

        // Combo was added to the repository but saving failed
        _unitOfWorkMock.Verify(u => u.ComboRepository.AddAsync(It.IsAny<Combo>()), Times.Once);
    }

    [Fact]
    public async Task CreateCombo_ReturnsError_WhenComboWithSameFoodIdsExists()
    {
        // Arrange
        var existingComboId = Guid.NewGuid();
        var existingCombo = new Combo
        {
            Id = existingComboId,
            Name = "Existing Combo",
            Content = "This is an existing combo",
            ComboFoods = new List<ComboFood>()
            {
                new ComboFood { FoodId = Guid.NewGuid() },
                new ComboFood { FoodId = Guid.NewGuid() },
            }
        };
        var createRequest = new CreateComboRequest
        {
            Name = "Test Combo",
            Content = "This is a test combo",
            FoodId = existingCombo.ComboFoods.Select(cf => cf.FoodId).ToList()
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetAllComboNotDeleted())
            .ReturnsAsync(new List<Combo>() { existingCombo });

        /*_mapper.Setup(m => m.Map<Combo>(It.IsAny<CreateComboRequest>()))
            .Returns(new Combo()); // Don't need specific details for this test*/

        // ImgurService and repository methods are not called in this scenario

        // Act
        var result = await _comboService.CreateCombo(createRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();

        result.Errors.Should().ContainSingle(e =>
            e.Code == ErrorCode.BadRequest &&
            e.Message.Contains("Combo is already exsit. Combo is: " + existingCombo.Name));
    }

    [Fact]
    public async Task UpdateCombo_ReturnsSuccess_WithValidData()
    {
        // Arrange
        var existingComboId = Guid.NewGuid();
        var existingCombo = new Combo
        {
            Id = existingComboId,
            Name = "Existing Combo",
            Content = "This is an existing combo",
            ComboFoods = new List<ComboFood>()
        };

        var updateRequest = new UpdateComboRequest
        {
            Name = "Updated Combo Name",
            Content = "Updated Combo Content",
            Image = null, // No image update
            FoodId = new List<Guid?>() { Guid.NewGuid(), Guid.NewGuid() }
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(existingComboId, x => x.ComboFoods))
            .ReturnsAsync(existingCombo);

        _imgurServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync((string)null); // No upload called

        _unitOfWorkMock.Setup(u => u.ComboFoodRepository.Remove(It.IsAny<ComboFood>()))
            .Verifiable(); // Expect existing ComboFoods to be removed

        _unitOfWorkMock.Setup(u => u.ComboFoodRepository.AddAsync(It.IsAny<ComboFood>()))
            .Verifiable(); // Expect new ComboFoods to be added

        _unitOfWorkMock.Setup(u => u.ComboRepository.Update(It.IsAny<Combo>()))
            .Verifiable();

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _comboService.UpdateCombo(existingComboId, updateRequest);

        // Assert
        result.IsError.Should().BeFalse();

        _unitOfWorkMock.Verify(u => u.ComboRepository.GetByIdAsync(existingComboId, x => x.ComboFoods), Times.Once);
        _unitOfWorkMock.Verify(u => u.ComboFoodRepository.Remove(It.IsAny<ComboFood>()),
            Times.Exactly(existingCombo.ComboFoods.Count));
        _unitOfWorkMock.Verify(u => u.ComboFoodRepository.AddAsync(It.IsAny<ComboFood>()),
            Times.Exactly(updateRequest.FoodId.Count(f => f.HasValue)));
        _unitOfWorkMock.Verify(u => u.ComboRepository.Update(It.IsAny<Combo>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCombo_ReturnsError_WhenComboNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateComboRequest
        {
            Name = "Updated Combo Name",
            Content = "Updated Combo Content"
        };

        _unitOfWorkMock.Setup(u =>
                u.ComboRepository.GetByIdAsync(nonExistentId, It.IsAny<Expression<Func<Combo, object>>>()))
            .ThrowsAsync(new NotFoundIdException());

        // Act
        var result = await _comboService.UpdateCombo(nonExistentId, updateRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();

        _unitOfWorkMock.Verify(
            u => u.ComboRepository.GetByIdAsync(nonExistentId, It.IsAny<Expression<Func<Combo, object>>>()),
            Times.Once);
        // No other methods called
    }

    [Fact]
    public async Task UpdateCombo_ReturnsError_OnSaveException()
    {
        // Arrange
        var existingComboId = Guid.NewGuid();
        var existingCombo = new Combo
        {
            Id = existingComboId,
            Name = "Existing Combo",
            Content = "This is an existing combo",
            ComboFoods = new List<ComboFood>()
        };

        var updateRequest = new UpdateComboRequest
        {
            Name = "Updated Combo Name",
            Content = "Updated Combo Content"
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(existingComboId, x => x.ComboFoods))
            .ReturnsAsync(existingCombo);

        // Mock other methods as needed (e.g., no image upload)

        _unitOfWorkMock.Setup(u => u.ComboFoodRepository.Remove(It.IsAny<ComboFood>()))
            .Verifiable(); // Expect existing ComboFoods to be removed

        _unitOfWorkMock.Setup(u => u.ComboFoodRepository.AddAsync(It.IsAny<ComboFood>()))
            .Verifiable(); // Expect new ComboFoods to be added

        _unitOfWorkMock.Setup(u => u.ComboRepository.Update(It.IsAny<Combo>()))
            .Verifiable();

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ThrowsAsync(new Exception("Saving changes failed"));

        // Act
        var result = await _comboService.UpdateCombo(existingComboId, updateRequest);

        // Assert
        result.IsError.Should().BeTrue();
        result.Payload.Should().BeNull();
        //result.Errors.Should().ContainSingle(e => e.Code == ErrorCode.Unknown && e.Message.Contains("Saving changes failed"));

        // Verifications (adjust based on your actual logic)
        _unitOfWorkMock.Verify(u => u.ComboRepository.GetByIdAsync(existingComboId, x => x.ComboFoods), Times.Once);
        _unitOfWorkMock.Verify(u => u.ComboFoodRepository.Remove(It.IsAny<ComboFood>()),
            Times.Exactly(existingCombo.ComboFoods.Count)); // May need adjustment
        _unitOfWorkMock.Verify(u => u.ComboRepository.Update(It.IsAny<Combo>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCombo_ReturnsError_WhenImageUploadFails()
    {
        // Arrange
        var existingComboId = Guid.NewGuid();
        var existingCombo = new Combo
        {
            Id = existingComboId,
            Name = "Existing Combo",
            Content = "This is an existing combo",
            ComboFoods = new List<ComboFood>()
        };

        var updateRequest = new UpdateComboRequest
        {
            Name = "Updated Combo Name",
            Content = "Updated Combo Content",
            Image = null // No actual image data
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(existingComboId, x => x.ComboFoods))
            .ReturnsAsync(existingCombo);

        _imgurServiceMock.Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>()))
            .ThrowsAsync(new Exception("Image upload failed"));

        // Act
        var result = await _comboService.UpdateCombo(existingComboId, updateRequest);

        // Assert
        result.Payload.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.ComboRepository.GetByIdAsync(existingComboId, x => x.ComboFoods), Times.Once);
    }
    
    [Fact]
    public async Task UpdateCombo_ReturnsSuccess_WithEmptyFoodId()
    {
        // Arrange
        var existingComboId = Guid.NewGuid();
        var existingCombo = new Combo
        {
            Id = existingComboId,
            Name = "Existing Combo",
            Content = "This is an existing combo",
            ComboFoods = new List<ComboFood>() { new ComboFood { Id = Guid.NewGuid() } }
        };

        var updateRequest = new UpdateComboRequest
        {
            Name = "Updated Combo Name",
            Content = "Updated Combo Content",
            Image = null,
            FoodId = new List<Guid?>() // Empty list
        };

        _unitOfWorkMock.Setup(u => u.ComboRepository.GetByIdAsync(existingComboId, x => x.ComboFoods))
            .ReturnsAsync(existingCombo);

        _unitOfWorkMock.Setup(u => u.ComboFoodRepository.Remove(It.IsAny<ComboFood>()))
            .Verifiable(); // Expect existing ComboFoods to be removed (all)

        _unitOfWorkMock.Setup(u => u.ComboFoodRepository.AddAsync(It.IsAny<ComboFood>()))
            .Verifiable(); // No new ComboFoods added

        _unitOfWorkMock.Setup(u => u.ComboRepository.Update(It.IsAny<Combo>()))
            .Verifiable();

        _unitOfWorkMock.Setup(u => u.SaveChangeAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _comboService.UpdateCombo(existingComboId, updateRequest);

        // Assert
        result.IsError.Should().BeFalse();
        // Verifications (adjust based on your actual logic)
        _unitOfWorkMock.Verify(u => u.ComboRepository.GetByIdAsync(existingComboId, x => x.ComboFoods), Times.Once);
        _unitOfWorkMock.Verify(u => u.ComboFoodRepository.Remove(It.IsAny<ComboFood>()), Times.Exactly(existingCombo.ComboFoods.Count));
        _unitOfWorkMock.Verify(u => u.ComboFoodRepository.AddAsync(It.IsAny<ComboFood>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.ComboRepository.Update(It.IsAny<Combo>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangeAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetComboPaginationAsync_ReturnsSuccess_WithEmptySearchTerm()
    {
        // Arrange (assuming 3 combos, 2nd page)
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var mockCombos = new List<Combo>()
        {
            new Combo
            {
                Id = id1,
                Name = "Combo 1",
                IsDeleted = false,
                ComboFoods = new List<ComboFood>()
                {
                    new ComboFood
                    {
                        Food = new Food
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Food 1",
                            Price = 10.50M,
                        },
                        ComboId = id1
                    }
                }
            },
            new Combo
            {
                Id = id2, 
                Name = "Combo 2", 
                IsDeleted = false,
                ComboFoods = new List<ComboFood>()
                {
                    new ComboFood
                    {
                        Food = new Food
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Food 2",
                            Price = 10.50M,
                        },
                        ComboId = id2
                    }
                }
            },
            new Combo
            {
                Id = id3,
                Name = "Combo 3", 
                IsDeleted = false,
                ComboFoods = new List<ComboFood>()
                {
                    new ComboFood
                    {
                        Food = new Food
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Food 3",
                            Price = 10.50M,
                        },
                        ComboId = id3
                    }
                }
            }
        };

        int pageIndex = 1; // Simulate 2nd page (0-based indexing)
        int pageSize = 2;
        int expectedTotalItems = mockCombos.Where(c => !c.IsDeleted).Count();

        _unitOfWorkMock.Setup(u => u.ComboRepository.ToPagination(pageIndex, pageSize, It.IsAny<Expression<Func<Combo, bool>>>(), It.IsAny<IncludeInfo<Combo>>()))
            .ReturnsAsync(new Pagination<Combo>
            {
                Items = mockCombos.Where(c => !c.IsDeleted).Skip(pageIndex * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = expectedTotalItems
            });

        /*_mapperMock.Setup(m => m.Map<List<ComboResponse>>(It.IsAny<List<Combo>>()))
            .Returns(new List<ComboResponse>() { new ComboResponse(), new ComboResponse() });*/
        /*var map = _mapper.Map<List<ComboResponse>>(mockCombos);*/

        // Act
        var result = await _comboService.GetComboPaginationAsync(null,1,2);

        // Assert
        result.IsError.Should().BeFalse();

        var pagination = result.Payload;
        pagination.Should().NotBeNull();

        pagination.PageIndex.Should().Be(pageIndex);
        pagination.PageSize.Should().Be(pageSize);
        pagination.TotalItemsCount.Should().Be(expectedTotalItems);
        // We no longer need to assert for TotalPagesCount since it's not calculated
        pagination.Items.Count.Should().Be(1); // 2nd page with 2 items
        pagination.Next.Should().BeFalse(); // Has next page
        pagination.Previous.Should().BeTrue(); // Has previous page (assuming page size is not equal to total items)
    }
}