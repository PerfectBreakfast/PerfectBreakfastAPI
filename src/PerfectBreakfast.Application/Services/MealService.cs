using System.Linq.Expressions;
using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MealModels.Request;
using PerfectBreakfast.Application.Models.MealModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class MealService : IMealService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;

    public MealService(IUnitOfWork unitOfWork, IMapper mapper,IClaimsService claimsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
    }
    
    public async Task<OperationResult<List<MealResponse>>> GetMeals()
    {
        var result = new OperationResult<List<MealResponse>>();
        try
        {
            var meals = await _unitOfWork.MealRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<MealResponse>>(meals);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<List<MealResponse>>> GetMealByWorker()
    {
        var result = new OperationResult<List<MealResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var companyInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Company,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Company)sp).MealSubscriptions,
                    sp => ((MealSubscription)sp).Meal
                }
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, companyInclude);
            var meals = user.Company.MealSubscriptions.Select(x => x.Meal);
            result.Payload = _mapper.Map<List<MealResponse>>(meals);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<MealResponse>> CreateMeal(CreateMealRequest request)
    {
        var result = new OperationResult<MealResponse>();
        try
        {
            var meal = _mapper.Map<Meal>(request); 
            var entity = await _unitOfWork.MealRepository.AddAsync(meal);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<MealResponse>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}