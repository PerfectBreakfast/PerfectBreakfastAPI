using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using System.Linq.Expressions;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.DailyOrder.Request;
using PerfectBreakfast.Application.Models.DailyOrder.Response;
using PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.Application.Services;

public class DailyOrderService : IDailyOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;
    private readonly ICurrentTime _currentTime;

    public DailyOrderService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, ICurrentTime currentTime)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        _currentTime = currentTime;
    }

    public async Task<OperationResult<DailyOrderResponse>> CreateDailyOrder(DailyOrderRequest dailyOrderRequest)
    {
        var result = new OperationResult<DailyOrderResponse>();
        try
        {
            var dailyOrder = _mapper.Map<DailyOrder>(dailyOrderRequest);
            dailyOrder.Status = DailyOrderStatus.Initial;
            dailyOrder.OrderQuantity = 0;
            dailyOrder.TotalPrice = 0;
            await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Company or Admin is not exist");
                return result;
            }

            result.Payload = _mapper.Map<DailyOrderResponse>(dailyOrder);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<Pagination<DailyOrderForPartnerResponse>>> GetDailyOrderProcessingByPartner(
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<DailyOrderForPartnerResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var partnerInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Partner,
                ThenIncludes =
                [
                    sp => ((Partner)sp).Companies,
                    sp => ((Company)sp).MealSubscriptions,
                    sp => ((MealSubscription)sp).Meal
                ]
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);

            // get các bữa ăn của từng công ty
            var mealSubscriptionIds = user.Partner.Companies
                .Where(c => !c.IsDeleted)
                .SelectMany(x => x.MealSubscriptions
                    .Where(c => !c.IsDeleted)
                    .Select(x => x.Id)).ToList();

            var dailyOrderPages =
                await _unitOfWork.DailyOrderRepository.ToPaginationForPartner(mealSubscriptionIds,pageIndex, pageSize);
            
            // Group DailyOrders by BookingDate and Company
            var dailyOrderResponses = dailyOrderPages.Items
                .GroupBy(d => new { CreationDate = DateOnly.FromDateTime(d.CreationDate) ,  d.BookingDate} )
                .OrderByDescending(group => group.Key.BookingDate)
                .Select(dateGroup => new DailyOrderForPartnerResponse(
                    dateGroup.Key.CreationDate,
                    dateGroup.Key.BookingDate, 
                    dateGroup
                        .Select(d => d.MealSubscription.Company)
                        .Distinct()
                        .Select(company => new CompanyForDailyOrderResponse(
                            company.Id,
                            company.Name,
                            company.Address,
                            dateGroup.Where(d => d.MealSubscription.CompanyId == company.Id)
                                .Select(d => new DailyOrderModelResponse(
                                    d.Id,
                                    d.MealSubscription.Meal.MealType,
                                    d.TotalPrice,
                                    d.OrderQuantity,
                                    d.Status.ToString()
                                )).ToList()
                        )).ToList()
                )).ToList();
            var totalItemsCount = dailyOrderResponses
                .SelectMany(dailyOrder => dailyOrder.Companies) 
                .SelectMany(company => company.DailyOrders) 
                .Count();
            result.Payload = new Pagination<DailyOrderForPartnerResponse>
            {
                PageIndex = dailyOrderPages.PageIndex,
                PageSize = dailyOrderPages.PageSize,
                TotalItemsCount = totalItemsCount,
                Items = dailyOrderResponses
            };
        }
        catch (NotFoundIdException)
        {
            result.AddUnknownError("Id is not exist");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<Pagination<DailyOrderForPartnerResponse>>> GetDailyOrderByPartner(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<DailyOrderForPartnerResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var partnerInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Partner,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Partner)sp).Companies,
                    sp => ((Company)sp).MealSubscriptions,
                    sp => ((MealSubscription)sp).Meal
                }
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);

            // get các bữa ăn của từng công ty
            var mealSubscriptionIds = user.Partner.Companies
                .Where(c => !c.IsDeleted)
                .SelectMany(x => x.MealSubscriptions
                    .Where(c => !c.IsDeleted)
                    .Select(x => x.Id)).ToList();

            var dailyOrderPages =
                await _unitOfWork.DailyOrderRepository.ToPaginationForAllStatus(mealSubscriptionIds,pageIndex, pageSize);
            
            // Group DailyOrders by BookingDate and Company
            var dailyOrderResponses = dailyOrderPages.Items
                .GroupBy(d => new { CreationDate = DateOnly.FromDateTime(d.CreationDate) ,  d.BookingDate} )
                .OrderByDescending(group => group.Key.BookingDate)
                .Select(dateGroup => new DailyOrderForPartnerResponse(
                    dateGroup.Key.CreationDate,
                    dateGroup.Key.BookingDate,
                    dateGroup
                        .Select(d => d.MealSubscription.Company)
                        .Distinct()
                        .Select(company => new CompanyForDailyOrderResponse(
                            company.Id,
                            company.Name,
                            company.Address,
                            dateGroup.Where(d => d.MealSubscription.CompanyId == company.Id)
                                .Select(d => new DailyOrderModelResponse(
                                    d.Id,
                                    d.MealSubscription.Meal.MealType,
                                    d.TotalPrice,
                                    d.OrderQuantity,
                                    d.Status.ToString()
                                )).ToList()
                        )).ToList()
                )).ToList();
            var totalItemsCount = dailyOrderResponses
                .SelectMany(dailyOrder => dailyOrder.Companies) 
                .SelectMany(company => company.DailyOrders) 
                .Count();
            result.Payload = new Pagination<DailyOrderForPartnerResponse>
            {
                PageIndex = dailyOrderPages.PageIndex,
                PageSize = dailyOrderPages.PageSize,
                TotalItemsCount = totalItemsCount,
                Items = dailyOrderResponses
            };
        }
        catch (NotFoundIdException)
        {
            result.AddUnknownError("Id is not exist");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<Pagination<DailyOrderForDeliveryResponse>>> GetDailyOrderProcessingByDelivery(
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var deliveryInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Delivery,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Delivery)sp).Companies,
                    sp => ((Company)sp).MealSubscriptions,
                    sp => ((MealSubscription)sp).Meal
                }
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, deliveryInclude);

            // Get các bữa ăn của từng công ty
            var mealSubscriptionIds =
                user.Delivery.Companies
                    .Where(c => !c.IsDeleted)
                    .SelectMany(x => x.MealSubscriptions.Where(c => !c.IsDeleted).Select(x => x.Id)).ToList();

            var dailyOrderPages =
                await _unitOfWork.DailyOrderRepository.ToPaginationForDelivery(mealSubscriptionIds,pageIndex, pageSize);
            
            var dailyOrderResponses = dailyOrderPages.Items
                .GroupBy(d => DateOnly.FromDateTime(d.BookingDate.ToDateTime(TimeOnly.MinValue)))
                .OrderByDescending(group => group.Key)
                .Select(dateGroup => new DailyOrderForDeliveryResponse(
                    dateGroup.Key,
                    dateGroup
                        .Select(d => d.MealSubscription.Company)
                        .Distinct()
                        .Select(company => new CompanyForDailyOrderResponse(
                            company.Id,
                            company.Name,
                            company.Address,
                            dateGroup.Where(d => d.MealSubscription.CompanyId == company.Id)
                                .Select(d => new DailyOrderModelResponse(
                                    d.Id,
                                    d.MealSubscription.Meal.MealType,
                                    d.TotalPrice,
                                    d.OrderQuantity,
                                    d.Status.ToString()
                                )).ToList()
                        )).ToList()
                )).ToList();
            var totalItemsCount = dailyOrderResponses
                .SelectMany(dailyOrder => dailyOrder.Companies) 
                .SelectMany(company => company.DailyOrders) 
                .Count();
            result.Payload = new Pagination<DailyOrderForDeliveryResponse>
            {
                PageIndex = dailyOrderPages.PageIndex,
                PageSize = dailyOrderPages.PageSize,
                TotalItemsCount = totalItemsCount, // lấy cứ mỗi một ngày là 1 ItemCount 
                Items = dailyOrderResponses
            };
        }
        catch (ArgumentNullException e)
        {
            result.AddError(ErrorCode.BadRequest,"User not found or user does not own delivery company");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Pagination<DailyOrderForDeliveryResponse>>> GetDailyOrderByDelivery(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var deliveryInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Delivery,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Delivery)sp).Companies,
                    sp => ((Company)sp).MealSubscriptions,
                    sp => ((MealSubscription)sp).Meal
                }
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, deliveryInclude);

            // Get các bữa ăn của từng công ty
            var mealSubscriptionIds =
                user.Delivery.Companies
                    .Where(c => !c.IsDeleted)
                    .SelectMany(x => x.MealSubscriptions.Where(c => !c.IsDeleted).Select(x => x.Id)).ToList();

            var dailyOrderPages =
                await _unitOfWork.DailyOrderRepository.ToPaginationForComplete(mealSubscriptionIds,pageIndex, pageSize);
            
            var dailyOrderResponses = dailyOrderPages.Items
                .GroupBy(d => DateOnly.FromDateTime(d.BookingDate.ToDateTime(TimeOnly.MinValue)))
                .OrderByDescending(group => group.Key)
                .Select(dateGroup => new DailyOrderForDeliveryResponse(
                    dateGroup.Key,
                    dateGroup
                        .Select(d => d.MealSubscription.Company)
                        .Distinct()
                        .Select(company => new CompanyForDailyOrderResponse(
                            company.Id,
                            company.Name,
                            company.Address,
                            dateGroup.Where(d => d.MealSubscription.CompanyId == company.Id)
                                .Select(d => new DailyOrderModelResponse(
                                    d.Id,
                                    d.MealSubscription.Meal.MealType,
                                    d.TotalPrice,
                                    d.OrderQuantity,
                                    d.Status.ToString()
                                )).ToList()
                        )).ToList()
                )).ToList();
            var totalItemsCount = dailyOrderResponses
                .SelectMany(dailyOrder => dailyOrder.Companies) 
                .SelectMany(company => company.DailyOrders) 
                .Count();
            result.Payload = new Pagination<DailyOrderForDeliveryResponse>
            {
                PageIndex = dailyOrderPages.PageIndex,
                PageSize = dailyOrderPages.PageSize,
                TotalItemsCount = totalItemsCount, // lấy cứ mỗi một ngày là 1 ItemCount 
                Items = dailyOrderResponses
            };
        }
        catch (ArgumentNullException e)
        {
            result.AddError(ErrorCode.BadRequest,"User not found or user does not own delivery company");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
    

    public async Task<OperationResult<bool>> CompleteDailyOrder(Guid id)
    {
        var result = new OperationResult<bool>();
        try
        {
            var now = _currentTime.GetCurrentTime();
            var mealInclude = new IncludeInfo<DailyOrder>
            {
                NavigationProperty = x => x.MealSubscription,
            };
            var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAndIncludeAsync(id, mealInclude);
            if(dailyOrder.MealSubscription?.EndTime > TimeOnly.FromTimeSpan(now.TimeOfDay))
            {
                result.AddError(ErrorCode.BadRequest, "Chưa tới giờ xác nhận đơn hàng");
                return result;
            }
            if (dailyOrder.Status == DailyOrderStatus.Delivering)
            {
                dailyOrder.Status = DailyOrderStatus.Complete;
                _unitOfWork.DailyOrderRepository.Update(dailyOrder);
            }
            else
            {
                result.AddError(ErrorCode.BadRequest, "Đơn hàng phải trong quá trình giao");
                return result;
            }
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            result.Payload = isSuccess;
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

    public async Task<OperationResult<List<DailyOrderStatisticResponse>>> GetDailyOrderForDownload(DateOnly fromDate, DateOnly toDate)
    {
        var result = new OperationResult<List<DailyOrderStatisticResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var partnerInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Partner,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Partner)sp).Companies,
                        sp => ((Company)sp).MealSubscriptions,
                            sp => ((MealSubscription)sp).Meal
                }
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);
            var dailyOrders =  await _unitOfWork.DailyOrderRepository.GetForStatistic();
            if (await _unitOfWork.UserManager.IsInRoleAsync(user, ConstantRole.PARTNER_ADMIN))
            {
                // get các bữa ăn của từng công ty
                var mealSubscriptionIds = user.Partner.Companies
                    .Where(c => !c.IsDeleted)
                    .SelectMany(x => x.MealSubscriptions
                        .Where(c => !c.IsDeleted)
                        .Select(x => x.Id)).ToList();
                dailyOrders = await _unitOfWork.DailyOrderRepository.GetByMeal(mealSubscriptionIds);
            }
            dailyOrders = dailyOrders.Where(d =>
                fromDate <= d.BookingDate && d.BookingDate <= toDate && d.Status != DailyOrderStatus.Initial && d.OrderQuantity > 0)
                .ToList();
            if (dailyOrders.Count == 0)
            {
                result.AddError(ErrorCode.BadRequest, "Ngày này không có đơn hàng nào");
                return result;
            }
            var dailyOrdersGroup = dailyOrders.GroupBy(x => x.BookingDate)
                .ToDictionary(x => x.Key, g => g.ToList());
            
            //Custom output
            var dailyOrderResponse = dailyOrdersGroup.Select(d =>
            {
                var bookingDate = d.Key;
                var creationDate = bookingDate.AddDays(-1);
                var companyForDailyOrderGroup = d.Value.GroupBy(d => new
                    {
                        Id = d.MealSubscription.Company.Id, 
                        Company = d.MealSubscription.Company.Name 
                    })
                    .ToDictionary(y => y.Key, g => g.ToList());
                var companyForDailyOrder = companyForDailyOrderGroup.Select(d =>
                {
                    var companyId = d.Key.Id;
                    var company = d.Key.Company;
                    var co = _unitOfWork.CompanyRepository
                        .GetByIdAsync(companyId, c => c.Partner, c => c.Delivery)
                        .Result;
                    var partner = co.Partner.Name;
                    var delivery = co.Delivery.Name;
                    var mealForDailyOrder = d.Value.Select(d =>
                    {
                        var meal = d.MealSubscription.Meal.MealType;
                        var time = new TimeOnly(1, 0, 0);
                        var foodForDailyOrder = new List<FoodForDailyOrder>();

                        var orders =  _unitOfWork.OrderRepository.GetOrderByDailyOrderId(d.Id).Result;
                        var orderDetails = orders
                            .SelectMany(order => order.OrderDetails)
                            .ToList();
                        foreach (var orderDetail in orderDetails)
                        {
                            if (orderDetail.ComboId != null)
                            {
                                var combo =  _unitOfWork.ComboRepository.GetComboFoodByIdAsync(orderDetail.ComboId).Result;
                                var foodEntities = combo.ComboFoods.Select(cf => cf.Food).ToList();
                                var comboForOrder = new FoodForDailyOrder
                                (
                                    combo.Name,
                                    $"{string.Join(", ", foodEntities.Select(food => food.Name))} - khẩu phầm combo",
                                    orderDetail.Quantity
                                );
                                foodForDailyOrder.Add(comboForOrder);
                            }
                            else if(orderDetail.FoodId != null)
                            {
                                var foodForOrder = new FoodForDailyOrder
                                (
                                    "None",
                                    orderDetail.Food.Name + "- khẩu phầm đơn lẻ",
                                    orderDetail.Quantity
                                );
                                foodForDailyOrder.Add(foodForOrder);
                            }
                        }
                        return new MealForDailyOrder(meal, time, foodForDailyOrder);
                    }).ToList(); 
                    
                    return new CompanyForDailyOrder(company, partner, delivery, mealForDailyOrder);
                }).ToList();

                return new DailyOrderStatisticResponse(creationDate, bookingDate,
                    companyForDailyOrder);
            }).ToList();

            result.Payload = dailyOrderResponse;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Pagination<DailyOrderForDeliveryResponse>>> GetDailyOrderPaginationAsync(
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<DailyOrderForDeliveryResponse>>();
        try
        {
            //Get tất cả các bữa ăn của từng công ty
            var mealSubscription =  _unitOfWork.MealSubscriptionRepository.FindAll(m => m.Company, m => m.Meal);
            var mealSubscriptionIds = mealSubscription.Where(m => !m.IsDeleted).Select(m => m.Id).ToList();
            var dailyOrderPages =
                await _unitOfWork.DailyOrderRepository.ToPagination(mealSubscriptionIds,pageIndex, pageSize);

            var dailyOrderResponses = dailyOrderPages.Items
                .GroupBy(d => DateOnly.FromDateTime(d.BookingDate.ToDateTime(TimeOnly.MinValue)))
                .OrderByDescending(group => group.Key)
                .Select(dateGroup => new DailyOrderForDeliveryResponse(
                    dateGroup.Key,
                    dateGroup
                        .Select(d => d.MealSubscription.Company)
                        .Distinct()
                        .Select(company => new CompanyForDailyOrderResponse(
                            company.Id,
                            company.Name,
                            company.Address,
                            dateGroup.Where(d => d.MealSubscription.CompanyId == company.Id)
                                .Select(d => new DailyOrderModelResponse(
                                    d.Id,
                                    d.MealSubscription.Meal.MealType,
                                    d.TotalPrice,
                                    d.OrderQuantity,
                                    d.Status.ToString()
                                )).ToList()
                        )).ToList()
                )).ToList();
            var totalItemsCount = dailyOrderResponses
                .SelectMany(dailyOrder => dailyOrder.Companies) 
                .SelectMany(company => company.DailyOrders) 
                .Count();
            result.Payload = new Pagination<DailyOrderForDeliveryResponse>
            {
                PageIndex = dailyOrderPages.PageIndex,
                PageSize = dailyOrderPages.PageSize,
                TotalItemsCount = totalItemsCount, // lấy cứ mỗi một ngày là 1 ItemCount 
                Items = dailyOrderResponses
            };
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<DailyOrderResponse>>> GetDailyOrders()
    {
        var result = new OperationResult<List<DailyOrderResponse>>();
        try
        {
            var dailyOrders = await _unitOfWork.DailyOrderRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<DailyOrderResponse>>(dailyOrders);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<DailyOrderResponse>> UpdateDailyOrder(Guid id,
        UpdateDailyOrderRequest updateDailyOrderRequest)
    {
        var result = new OperationResult<DailyOrderResponse>();
        try
        {
            var dailyOrderEntity = await _unitOfWork.DailyOrderRepository.GetByIdAsync(id);
            _mapper.Map(updateDailyOrderRequest, dailyOrderEntity);
            _unitOfWork.DailyOrderRepository.Update(dailyOrderEntity);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<DailyOrderResponse>(dailyOrderEntity);
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