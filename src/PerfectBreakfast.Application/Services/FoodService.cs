﻿using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services
{
    public class FoodService : IFoodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FoodService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<FoodResponse>> CreateFood(CreateFoodRequestModels requestModel)
        {
            var result = new OperationResult<FoodResponse>();
            try
            {
                // map model to Entity
                var food = _mapper.Map<Food>(requestModel);
                // Add to DB
                var entity = await _unitOfWork.FoodRepository.AddAsync(food);
                // save change 
                await _unitOfWork.SaveChangeAsync();
                // map model to response
                result.Payload = _mapper.Map<FoodResponse>(entity);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<FoodResponse>>> GetAllFoods()
        {
            var result = new OperationResult<List<FoodResponse>>();
            try
            {
                var foods = await _unitOfWork.FoodRepository.GetAllAsync();
                result.Payload = _mapper.Map<List<FoodResponse>>(foods);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<FoodResponeCategory>> GetFoodById(Guid foodId)
        {
            var result = new OperationResult<FoodResponeCategory>();
            try
            {
                var food = await _unitOfWork.FoodRepository.FindSingleAsync(o => o.Id == foodId, o=>o.Category);
                if (food is null)
                {
                    result.AddUnknownError("Id is not exist");
                    return result;
                }

                var category = _mapper.Map<CategoryResponse>(food.Category);
                var o = _mapper.Map<FoodResponeCategory>(food);
                o.CategoryResponse = category;
                result.Payload = o;
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<Pagination<FoodResponse>>> GetFoodPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<FoodResponse>>();
            try
            {
                var foods = await _unitOfWork.FoodRepository.ToPagination(pageIndex, pageSize);
                result.Payload = _mapper.Map<Pagination<FoodResponse>>(foods);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<FoodResponse>> RemoveFood(Guid foodId)
        {
            var result = new OperationResult<FoodResponse>();
            try
            {
                // find supplier by ID
                var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
                // Remove
                var entity = _unitOfWork.FoodRepository.Remove(food);
                // saveChange
                await _unitOfWork.SaveChangeAsync();
                // map entity to SupplierResponse
                result.Payload = _mapper.Map<FoodResponse>(entity);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<FoodResponse>> UpdateFood(Guid foodId, UpdateFoodRequestModels requestModel)
        {
            var result = new OperationResult<FoodResponse>();
            try
            {
                // find supplier by ID
                var food = await _unitOfWork.FoodRepository.GetByIdAsync(foodId);
                // map from requestModel => supplier
                _mapper.Map(requestModel, food);
                // update
                _unitOfWork.FoodRepository.Update(food);
                // saveChange
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<FoodResponse>(food);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}
