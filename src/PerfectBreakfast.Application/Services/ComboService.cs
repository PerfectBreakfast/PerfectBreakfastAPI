using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
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
                var combo = _mapper.Map<Combo>(createComboRequest);
                var comboFood = _mapper.Map<List<ComboFood?>>(createComboRequest.ComboFoodRequests);
                combo.ComboFoods = comboFood;
                combo.Image = await _imgurService.UploadImageAsync(createComboRequest.Image);
                await _unitOfWork.ComboRepository.AddAsync(combo);
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<ComboResponse>(combo);
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
            catch (NotFoundIdException)
            {
                result.AddUnknownError("Id is not exsit");
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
                result.AddUnknownError("Id is not exsit");
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
                if (combo is null)
                {
                    result.AddUnknownError("Id is not exsit");
                    return result;
                }
                var foodEntities = combo.ComboFoods.Select(cf => cf.Food).ToList();
                decimal totalFoodPrice = foodEntities.Sum(food => food.Price);
                var listComboFood = _mapper.Map<List<FoodResponse?>>(foodEntities);
                var co = _mapper.Map<ComboResponse>(combo);
                co.Foods = $"{string.Join(", ", foodEntities.Select(food => food.Name))}";
                co.FoodResponses = listComboFood;
                co.comboPrice = totalFoodPrice;
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
                var combos = await _unitOfWork.ComboRepository.GetAllCombo();
                List<ComboResponse> comboResponses = new List<ComboResponse>();
                foreach (var combo in combos)
                {
                    var foodEntities = combo.ComboFoods.Select(cf => cf.Food).ToList();
                    decimal totalFoodPrice = foodEntities.Sum(food => food.Price);
                    var co = _mapper.Map<ComboResponse>(combo);
                    co.Foods = $"{string.Join(", ", foodEntities.Select(food => food.Name))}";
                    co.comboPrice = totalFoodPrice;
                    comboResponses.Add(co);
                }

                var comboPagination = await _unitOfWork.ComboRepository.GetAllCombosWithPaginationAsync(pageIndex, pageSize, comboResponses);
                result.Payload = _mapper.Map<Pagination<ComboResponse>>(comboPagination);
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
                var combos = await _unitOfWork.ComboRepository.GetAllCombo();
                List<ComboResponse> comboResponses = new List<ComboResponse>();
                foreach (var combo in combos)
                {
                    var foodEntities = combo.ComboFoods.Select(cf => cf.Food).ToList();
                    decimal totalFoodPrice = foodEntities.Sum(food => food.Price);
                    var co = _mapper.Map<ComboResponse>(combo);
                    co.Foods = $"{string.Join(", ", foodEntities.Select(food => food.Name))}";
                    co.comboPrice = totalFoodPrice;
                    comboResponses.Add(co);
                }
                result.Payload = comboResponses;
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
                var comboEntity = await _unitOfWork.ComboRepository.GetByIdAsync(id);
                _mapper.Map(comboRequest, comboEntity);
                _unitOfWork.ComboRepository.Update(comboEntity);
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<ComboResponse>(comboEntity);
            }
            catch (NotFoundIdException)
            {
                result.AddUnknownError("Id is not exsit");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}
