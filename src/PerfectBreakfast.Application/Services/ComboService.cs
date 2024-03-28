using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ComboModels.Request;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace PerfectBreakfast.Application.Services;

public class ComboService : IComboService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImgurService _imgurService;

    public ComboService(IUnitOfWork unitOfWork, IMapper mapper, IImgurService imgurService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imgurService = imgurService;
    }

    public async Task<OperationResult<ComboResponse>> CreateCombo(CreateComboRequest createComboRequest)
    {
        var result = new OperationResult<ComboResponse>();
        try
        {
            var combos = await _unitOfWork.ComboRepository.GetAllCombo();
            var combo = _mapper.Map<Combo>(createComboRequest);
            var comboFoods = createComboRequest.FoodId
                .Select(foodId => new ComboFood { FoodId = foodId })
                .ToList();

            foreach (var existingCombo in combos)
            {
                var existingComboFoods = existingCombo.ComboFoods;

                if (existingComboFoods.Count() == comboFoods.Count)
                {
                    // Check if FoodId values are the same for corresponding ComboFoods
                    if (existingComboFoods.All(ef => comboFoods.Any(cf => cf.FoodId == ef.FoodId)))
                    {
                        // The sets of ComboFoods are the same, return or handle as needed
                        result.AddError(ErrorCode.BadRequest,
                            "Combo is already exsit. Combo is: " + existingCombo.Name);
                        return result;
                    }
                }
            }

            combo.ComboFoods = comboFoods;
            combo.Image = await _imgurService.UploadImageAsync(createComboRequest.Image);
            await _unitOfWork.ComboRepository.AddAsync(combo);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Food is not exist");
                return result;
            }

            result.Payload = _mapper.Map<ComboResponse>(combo);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<ComboResponse>> DeleteCombo(Guid id)
    {
        var result = new OperationResult<ComboResponse>();
        try
        {
            var combo = await _unitOfWork.ComboRepository.GetByIdAsync(id);
            _unitOfWork.ComboRepository.SoftRemove(combo);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (NotFoundIdException)
        {
            result.AddError(ErrorCode.NotFound, "Id is not exist");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<ComboDetailResponse>> GetCombo(Guid id)
    {
        var result = new OperationResult<ComboDetailResponse>();
        try
        {
            var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(id);
            if (combo is null)
            {
                result.AddError(ErrorCode.NotFound, "Id is not exist");
                return result;
            }

            var foods = combo.ComboFoods.Select(cf => cf.Food).ToList();
            var co = _mapper.Map<ComboDetailResponse>(combo);
            co.FoodResponses = _mapper.Map<List<FoodResponseCategory?>>(foods);
            result.Payload = co;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<Pagination<ComboResponse>>> GetComboPaginationAsync(string? searchTerm,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<ComboResponse>>();
        try
        {
            // xác định các thuộc tính include và theninclude 
            var comboFoodInclude = new IncludeInfo<Combo>
            {
                NavigationProperty = c => c.ComboFoods,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    cf => ((ComboFood)cf).Food,
                }
            };

            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<Combo, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm)
                ? (x => !x.IsDeleted)
                : (x => x.Name.ToLower().Contains(searchTerm.ToLower()) && !x.IsDeleted);

            // lấy page combo 
            var pagedCombos =
                await _unitOfWork.ComboRepository.ToPagination(pageIndex, pageSize, searchPredicate, comboFoodInclude);
            // Chuyển đổi từ Combo sang ComboResponse
            var comboResponses = _mapper.Map<List<ComboResponse>>(pagedCombos.Items);
            result.Payload = new Pagination<ComboResponse>
            {
                PageIndex = pagedCombos.PageIndex,
                PageSize = pagedCombos.PageSize,
                TotalItemsCount = pagedCombos.TotalItemsCount,
                Items = comboResponses
            };
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<List<ComboResponse>>> GetCombos()
    {
        var result = new OperationResult<List<ComboResponse>>();
        try
        {
            var combos = await _unitOfWork.ComboRepository.FindAll(c => !c.IsDeleted).ToListAsync();
            result.Payload = _mapper.Map<List<ComboResponse>>(combos);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<ComboResponse>> UpdateCombo(Guid id, UpdateComboRequest updateComboRequest)
    {
        var result = new OperationResult<ComboResponse>();
        try
        {
            var comboEntity = await _unitOfWork.ComboRepository.GetByIdAsync(id, x => x.ComboFoods);
            //_mapper.Map(updateComboRequest, comboEntity);
            comboEntity.Name = updateComboRequest.Name ?? comboEntity.Name;
            comboEntity.Content = updateComboRequest.Content ?? comboEntity.Content;
            if (updateComboRequest.Image is not null)
            {
                comboEntity.Image = await _imgurService.UploadImageAsync(updateComboRequest.Image);
            }

            // Xử lý danh sách FoodId mới
            if (updateComboRequest.FoodId is not null)
            {
                var existingComboFoods = comboEntity.ComboFoods.ToList();
                foreach (var comboFood in existingComboFoods)
                {
                    _unitOfWork.ComboFoodRepository.Remove(comboFood);
                }

                // Tạo mới mối quan hệ ComboFood với danh sách FoodId mới
                foreach (var foodId in updateComboRequest.FoodId.Where(id => id.HasValue))
                {
                    var newComboFood = new ComboFood
                    {
                        ComboId = comboEntity.Id,
                        FoodId = foodId.Value,
                    };
                    await _unitOfWork.ComboFoodRepository.AddAsync(newComboFood);
                }
            }

            _unitOfWork.ComboRepository.Update(comboEntity);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (NotFoundIdException)
        {
            result.AddError(ErrorCode.NotFound, "Id is not exist");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}