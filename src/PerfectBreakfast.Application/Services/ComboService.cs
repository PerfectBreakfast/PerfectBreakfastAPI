﻿using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ComboModels.Request;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services
{
    public class ComboService : IComboService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ComboService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<ComboResponse>> CreateCombo(CreateComboRequest createComboRequest)
        {
            var result = new OperationResult<ComboResponse>();
            try
            {
                var combo = _mapper.Map<Combo>(createComboRequest);
                var comboFood = _mapper.Map<ICollection<ComboFood?>>(createComboRequest.ComboFoodRequests);
                combo.ComboFoods = comboFood;
                await _unitOfWork.ComboRepository.AddAsync(combo);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);

            }
            return result;
        }

        public async Task<OperationResult<ComboResponse>> Delete(Guid id)
        {
            var result = new OperationResult<ComboResponse>();
            try
            {
                var combo = await _unitOfWork.ComboRepository.GetByIdAsync(id);
                _unitOfWork.ComboRepository.Remove(combo);
                await _unitOfWork.SaveChangeAsync();
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
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<ComboResponse>> GetCombo(Guid id)
        {
            var result = new OperationResult<ComboResponse>();
            try
            {
                var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(id);
                var foodEntities = combo.ComboFoods.Select(cf => cf.Food).ToList();
                var listComboFood = _mapper.Map<List<FoodResponse?>>(foodEntities);
                var co = _mapper.Map<ComboResponse>(combo);
                co.FoodResponses = listComboFood;
                result.Payload = co;
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<Pagination<ComboResponse>>> GetComboPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<ComboResponse>>();
            try
            {
                var combo = await _unitOfWork.ComboRepository.ToPagination(pageIndex, pageSize);
                result.Payload = _mapper.Map<Pagination<ComboResponse>>(combo);
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
                var combo = await _unitOfWork.ComboRepository.GetAllAsync();
                result.Payload = _mapper.Map<List<ComboResponse>>(combo);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<ComboResponse>> UpdateCombo(Guid id, ComboRequest comboRequest)
        {
            var result = new OperationResult<ComboResponse>();
            try
            {
                var combo = _mapper.Map<Combo>(comboRequest);
                combo.Id = id;
                _unitOfWork.ComboRepository.Update(combo);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}