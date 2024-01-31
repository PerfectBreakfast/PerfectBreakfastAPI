using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Services
{
    public class FoodService : IFoodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImgurService _imgurService;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _currentTime;
        public FoodService(IUnitOfWork unitOfWork, IMapper mapper, IImgurService imgurService, IClaimsService claimsService, ICurrentTime currentTime)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imgurService = imgurService;
            _claimsService = claimsService;
            _currentTime = currentTime;
        }

        public async Task<OperationResult<FoodResponse>> CreateFood(CreateFoodRequestModels requestModel)
        {
            var result = new OperationResult<FoodResponse>();
            try
            {
                // map model to Entity
                var food = _mapper.Map<Food>(requestModel);
                food.Image = await _imgurService.UploadImageAsync(requestModel.Image);
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
                var food = await _unitOfWork.FoodRepository.FindSingleAsync(o => o.Id == foodId, o => o.Category);
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

        public async Task<OperationResult<Pagination<FoodResponse>>> GetFoodPaginationAsync(string? searchTerm, int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<FoodResponse>>();
            try
            {
                // Tạo biểu thức tìm kiếm (predicate)
                Expression<Func<Food, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm)
                    ? null
                    : (x => x.Name.ToLower().Contains(searchTerm.ToLower()));
                var foods = await _unitOfWork.FoodRepository.ToPagination(pageIndex, pageSize, searchPredicate);
                result.Payload = _mapper.Map<Pagination<FoodResponse>>(foods);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<TotalFoodResponse>>> GetFoodsForPartner()
        {
            var userId = _claimsService.GetCurrentUserId;
            var result = new OperationResult<List<TotalFoodResponse>>();
            try
            {
                var now = _currentTime.GetCurrentTime();
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                var managementUnits = await _unitOfWork.PartnerRepository.GetManagementUnitsByToday(now);
                var managementUnit = managementUnits.SingleOrDefault(m => m.Id == user.PartnerId);

                if (managementUnit == null)
                {
                    result.AddUnknownError("managementUnit does not exsit");
                    return result;
                }

                // Lấy danh sách các công ty thuộc MU
                var companies = managementUnit.Companies;
                var foodCounts = new Dictionary<string, int>();

                // xử lý mỗi cty
                foreach (var company in companies)
                {
                    // Lấy daily order
                    var dailyOrder = company.DailyOrders.SingleOrDefault(x => x.CreationDate.Date == now.Date);

                    // Lấy chi tiết các order detail
                    var orders = await _unitOfWork.OrderRepository.GetOrderByDailyOrderId(dailyOrder.Id);
                    var orderDetails = orders.SelectMany(order => order.OrderDetails).ToList();

                    // Đếm số lượng từng loại food
                    foreach (var orderDetail in orderDetails)
                    {
                        if (orderDetail.Combo != null)
                        {
                            // Nếu là combo thì lấy chi tiết các food trong combo
                            var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(orderDetail.Combo.Id);
                            var comboFoods = combo.ComboFoods;

                            // Với mỗi food trong combo, cộng dồn số lượng
                            foreach (var comboFood in comboFoods)
                            {
                                var foodName = comboFood.Food.Name;
                                // Kiểm tra xem thức ăn đã tồn tại trong foodCounts chưa
                                if (foodCounts.ContainsKey(foodName))
                                {
                                    // Nếu đã tồn tại, cộng dồn số lượng mới vào số lượng hiện có
                                    foodCounts[foodName] += orderDetail.Quantity;
                                }
                                else
                                {
                                    // Nếu chưa tồn tại, thêm mới vào foodCounts
                                    foodCounts[foodName] = orderDetail.Quantity;
                                }
                            }
                        }
                        else if (orderDetail.Food != null)
                        {
                            // Xử lý order detail là food đơn lẻ
                            var foodName = orderDetail.Food.Name;
                            // Kiểm tra xem thức ăn đã tồn tại trong foodCounts chưa
                            if (foodCounts.ContainsKey(foodName))
                            {
                                // Nếu đã tồn tại, cộng dồn số lượng mới vào số lượng hiện có
                                foodCounts[foodName] += orderDetail.Quantity;
                            }
                            else
                            {
                                // Nếu chưa tồn tại, thêm mới vào foodCounts
                                foodCounts[foodName] = orderDetail.Quantity;
                            }
                        }
                    }
                }
                // Tạo danh sách totalFoodList từ foodCounts
                var totalFoodList = foodCounts.Select(pair => new TotalFoodResponse { Name = pair.Key, Quantity = pair.Value }).ToList();
                result.Payload = totalFoodList;
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
                food.Image = await _imgurService.UploadImageAsync(requestModel.Image);
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
