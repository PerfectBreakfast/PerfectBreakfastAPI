using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.FoodModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services;

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
                if (requestModel.FoodStatus is not (0 or 1))
                {
                    result.AddError(ErrorCode.BadRequest, "Food status must be 0 or 1");
                }
                
                // map model to Entity
                var food = _mapper.Map<Food>(requestModel);
                food.FoodStatus = requestModel.FoodStatus == 0 ? FoodStatus.Combo : FoodStatus.Retail;
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
                foods = foods.Where(s => s.IsDeleted == false).ToList();
                result.Payload = _mapper.Map<List<FoodResponse>>(foods);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<FoodResponse>>> GetFoodByFoodStatus(FoodStatus status)
        {
            var result = new OperationResult<List<FoodResponse>>();
            try
            {
                var foods = await _unitOfWork.FoodRepository.FindAll(x => x.FoodStatus == status && !x.IsDeleted).ToListAsync();
                result.Payload = _mapper.Map<List<FoodResponse>>(foods);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<FoodResponseCategory>> GetFoodById(Guid foodId)
        {
            var result = new OperationResult<FoodResponseCategory>();
            try
            {
                var food = await _unitOfWork.FoodRepository.FindSingleAsync(o => o.Id == foodId, o => o.Category);
                if (food is null)
                {
                    result.AddError(ErrorCode.NotFound,"Id is not exist");
                    return result;
                }
                result.Payload = _mapper.Map<FoodResponseCategory>(food);
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
                    ? (x => !x.IsDeleted)
                    : (x => x.Name.ToLower().Contains(searchTerm.ToLower()) && !x.IsDeleted);
                var foods = await _unitOfWork.FoodRepository.ToPagination(pageIndex, pageSize, searchPredicate);
                result.Payload = _mapper.Map<Pagination<FoodResponse>>(foods);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<TotalFoodForPartnerResponse>> GetFoodsForPartner(Guid dailyOrderId)
        {
            var userId = _claimsService.GetCurrentUserId;
            var result = new OperationResult<TotalFoodForPartnerResponse>();
            try
            {
                var partnerInclude = new IncludeInfo<User>
                {
                    NavigationProperty = x => x.Partner,
                    ThenIncludes = [sp => ((Partner)sp).Companies]
                };
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);
                
                if (user.Partner == null)
                {
                    result.AddError(ErrorCode.NotFound, "partner does not exist");
                    return result;
                }
                        
                // Lấy daily order
                var mealInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.MealSubscription,
                    ThenIncludes = [sp => ((MealSubscription)sp).Meal]
                };
                var companyInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.MealSubscription,
                    ThenIncludes = [sp => ((MealSubscription)sp).Company]
                };
                var foodInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.Orders,
                    ThenIncludes =
                    [
                        sp => ((Order)sp).OrderDetails,
                            sp => ((OrderDetail)sp).Food
                    ]
                };
                var comboInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.Orders,
                    ThenIncludes =
                    [
                        sp => ((Order)sp).OrderDetails,
                            sp => ((OrderDetail)sp).Combo,
                                sp => ((Combo)sp).ComboFoods,
                                    sp => ((ComboFood)sp).Food
                    ]
                };
                
                var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAndIncludeAsync(dailyOrderId, mealInclude, foodInclude, companyInclude, comboInclude);
                
                // Kiểm tra xem công ty có trong danh sách đối tác không
                var companyFound = user.Partner.Companies.Any(company => company.Id == dailyOrder.MealSubscription.CompanyId);

                // Nếu công ty không được tìm thấy, thêm lỗi vào kết quả
                if (!companyFound)
                {
                    result.AddError(ErrorCode.BadRequest, "Company doesn't have this daily order");
                    return result;
                }
                
                // Lấy chi tiết các order detail
                var orderDetails = dailyOrder.Orders.SelectMany(order => order.OrderDetails).ToList();
                
                //Đếm số lượng món theo khẩu phần
                var foodCounts = new Dictionary<(Guid, string), int>();
                foreach (var orderDetail in orderDetails)
                {
                    if (orderDetail.Combo != null)
                    {
                        // Xử lý combo
                        foreach (var comboFood in orderDetail.Combo.ComboFoods)
                        {
                            var food = comboFood.Food;
                            var key = (food.Id, $"{food.Name} - khẩu phần combo");
                            // Tất cả Food trong Combo sẽ được xử lý như khẩu phần combo
                            if (foodCounts.ContainsKey(key))
                            {
                                foodCounts[key] += orderDetail.Quantity;
                            }
                            else
                            {
                                foodCounts[key] = orderDetail.Quantity;
                            }
                        }
                    }
                    else if (orderDetail.Food != null)
                    {
                        var food = orderDetail.Food;
                        // Food đơn lẻ với Status là Retail sẽ được xử lý như khẩu phần đơn lẻ
                        var key = (food.Id, $"{food.Name} - khẩu phần đơn lẻ");
                        if (foodCounts.ContainsKey(key))
                        {
                            foodCounts[key] += orderDetail.Quantity;
                        }
                        else
                        {
                            foodCounts[key] = orderDetail.Quantity;
                        }
                    }
                }
                
                // Tạo danh sách totalFoodList từ foodCounts
                var totalFoodList = foodCounts.Select(pair =>
                    new TotalFoodResponse
                {
                    Id = pair.Key.Item1, // Truy cập Food thông qua Item1 của tuple
                    Name = $"{pair.Key.Item2}", // Kết hợp tên Food với loại
                    Quantity = pair.Value
                }).ToList();
                
                var totalFoodForPartner = new TotalFoodForPartnerResponse()
                {
                    DailyOrderId = dailyOrder.Id,
                    Meal = dailyOrder.MealSubscription.Meal.MealType,
                    CompanyName = dailyOrder.MealSubscription.Company.Name,
                    Phone = dailyOrder.MealSubscription.Company.PhoneNumber,
                    Status = dailyOrder.Status.ToString(),
                    TotalFoodResponses = totalFoodList
                };
                result.Payload = totalFoodForPartner;
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

        public async Task<OperationResult<TotalFoodForPartnerResponse>> GetFoodsForDelivery(Guid dailyOrderId)
        {
            var userId = _claimsService.GetCurrentUserId;
            var result = new OperationResult<TotalFoodForPartnerResponse>();
            try
            {
                var deliveryInclude = new IncludeInfo<User>
                {
                    NavigationProperty = x => x.Delivery,
                    ThenIncludes = [sp => ((Delivery)sp).Companies]
                };
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, deliveryInclude);

                if (user.Delivery == null)
                {
                    result.AddError(ErrorCode.NotFound, "Delivery does not exist");
                    return result;
                }
                        
                // Lấy daily order
                var mealInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.MealSubscription,
                    ThenIncludes = [sp => ((MealSubscription)sp).Meal]
                };
                var companyInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.MealSubscription,
                    ThenIncludes = [sp => ((MealSubscription)sp).Company]
                };
                var foodInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.Orders,
                    ThenIncludes =
                    [
                        sp => ((Order)sp).OrderDetails,
                            sp => ((OrderDetail)sp).Food
                    ]
                };
                var comboInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.Orders,
                    ThenIncludes =
                    [
                        sp => ((Order)sp).OrderDetails,
                            sp => ((OrderDetail)sp).Combo,
                                sp => ((Combo)sp).ComboFoods,
                                    sp => ((ComboFood)sp).Food
                    ]
                };
                
                var dailyOrder = await _unitOfWork.DailyOrderRepository
                    .GetByIdAndIncludeAsync(dailyOrderId, mealInclude, companyInclude, comboInclude, foodInclude);

                // Kiểm tra xem công ty có trong danh sách đối tác không
                var companyFound = user.Delivery.Companies.Any(company => company.Id == dailyOrder.MealSubscription.CompanyId);

                // Nếu công ty không được tìm thấy, thêm lỗi vào kết quả
                if (!companyFound)
                {
                    result.AddError(ErrorCode.BadRequest, "Company doesn't have this daily order");
                    return result;
                }
                
                // Lấy chi tiết các order detail
                var orderDetails = dailyOrder.Orders.SelectMany(order => order.OrderDetails).ToList();

                // Đếm số lượng từng loại food
                var foodCounts = new Dictionary<(Guid, string), int>();
                foreach (var orderDetail in orderDetails)
                {
                    if (orderDetail.Combo != null)
                    {
                        // Xử lý combo
                        foreach (var comboFood in orderDetail.Combo.ComboFoods)
                        {
                            var food = comboFood.Food;
                            // Tất cả Food trong Combo sẽ được xử lý như khẩu phần combo
                            var key = (food.Id, $"{food.Name} - khẩu phần combo");
                            if (foodCounts.ContainsKey(key))
                            {
                                foodCounts[key] += orderDetail.Quantity;
                            }
                            else
                            {
                                foodCounts[key] = orderDetail.Quantity;
                            }
                        }
                    }
                    else if (orderDetail.Food != null)
                    {
                        var food = orderDetail.Food;
                        // Food đơn lẻ với Status là Retail sẽ được xử lý như khẩu phần đơn lẻ
                        var key = (food.Id, $"{food.Name} - khẩu phần đơn lẻ");
                        if (foodCounts.ContainsKey(key))
                        {
                            foodCounts[key] += orderDetail.Quantity;
                        }
                        else
                        {
                            foodCounts[key] = orderDetail.Quantity;
                        }
                    }
                }
                // Tạo danh sách totalFoodList từ foodCounts
                var totalFoodList = foodCounts.Select(pair => new TotalFoodResponse
                {
                    Id = pair.Key.Item1, // Truy cập Food thông qua Item1 của tuple
                    Name = $"{pair.Key.Item2}", // Kết hợp tên Food với loại
                    Quantity = pair.Value
                }).ToList();
                var totalFoodForPartner = new TotalFoodForPartnerResponse()
                {
                    DailyOrderId = dailyOrder.Id,
                    Meal = dailyOrder.MealSubscription.Meal.MealType,
                    CompanyName = dailyOrder.MealSubscription.Company.Name,
                    Phone = dailyOrder.MealSubscription.Company.PhoneNumber,
                    Status = dailyOrder.Status.ToString(),
                    TotalFoodResponses = totalFoodList
                };
                result.Payload = totalFoodForPartner;
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

        public async Task<OperationResult<List<FoodResponse>>> GetFoodForSupplier(Guid id)
        {
            var result = new OperationResult<List<FoodResponse>>();
            try
            {
                var food = await _unitOfWork.FoodRepository.GetFoodForSupplier(id);
                result.Payload = _mapper.Map<List<FoodResponse>>(food);
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
                food.Name = requestModel.Name ?? food.Name;
                food.Price = requestModel.Price ?? food.Price;
                if (requestModel.Image is not null)
                {
                    food.Image = await _imgurService.UploadImageAsync(requestModel.Image);
                }
                if (requestModel.FoodStatus is not null)
                {
                    if (requestModel.FoodStatus is not (0 or 1))
                    {
                        result.AddError(ErrorCode.BadRequest, "Food status must be 0 or 1");
                    }
                    food.FoodStatus = requestModel.FoodStatus == 0 ? FoodStatus.Combo : FoodStatus.Retail;
                }
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
