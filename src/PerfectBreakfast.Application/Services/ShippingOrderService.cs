using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DailyOrder.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services;

public class ShippingOrderService : IShippingOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;


    public ShippingOrderService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
    }

    // get all shipper with dailyOder Detail
    public async Task<OperationResult<List<ShippingOrderDTO>>> GetAllShippingOrdersWithDetails()
    {
        var result = new OperationResult<List<ShippingOrderDTO>>();
        try
        {
            var shippingOrders = await _unitOfWork.ShippingOrderRepository
                .GetAllWithDailyOrdersAsync(); // Assuming this correctly fetches ShippingOrder including DailyOrder

            var shippingOrderDTOs = shippingOrders.Select(so => new ShippingOrderDTO
            {
                ShipperId = so.ShipperId,
                DailyOrder = _mapper.Map<DailyOrderResponse>(so.DailyOrder) // Direct mapping without Select
            }).ToList();

            result.Payload = shippingOrderDTOs;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<ShippingOrderResponse>>> CreateShippingOrder(
        CreateShippingOrderRequest requestModel)
    {
        var result = new OperationResult<List<ShippingOrderResponse>>();
        var list = new List<ShippingOrder>();
        try
        {
            var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync(requestModel.DailyOrderId);
            if (dailyOrder.Status != DailyOrderStatus.Processing && dailyOrder.Status != DailyOrderStatus.Cooking)
            {
                result.AddError(ErrorCode.BadRequest,
                    $"Trạng thái đơn hàng không đúng {dailyOrder.Status.ToString()}");
                return result;
            }

            // Check for duplicate shipping order
            bool exists = await _unitOfWork.ShippingOrderRepository.ExistsWithDailyOrderAndShippers(
                requestModel.DailyOrderId, requestModel.ShipperIds);
            if (exists)
            {
                result.AddError(ErrorCode.BadRequest,
                    "A shipping order with the same DailyOrderId and one of the ShipperIds already exists.");
                return result;
            }

            list.AddRange(requestModel.ShipperIds.Select(shipperId => new ShippingOrder
            {
                ShipperId = shipperId,
                DailyOrderId = requestModel.DailyOrderId,
                Status = ShippingStatus.Pending
            }));
            await _unitOfWork.ShippingOrderRepository.AddRangeAsync(list);
            await _unitOfWork.SaveChangeAsync();
            return result;
        }
        catch (NotFoundIdException)
        {
            result.AddError(ErrorCode.NotFound, "ID was not found.");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<ShippingOrderForShipperResponse>>> GetShippingOrderTodayByDeliveryStaff(DateTime time)
    {
        var result = new OperationResult<List<ShippingOrderForShipperResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            // Hàm này còn đang sửa tiêp cho nagyf mai 
            var date = DateOnly.FromDateTime(time);
            var shippingOrders = await _unitOfWork.ShippingOrderRepository.GetShippingOrderTodayByShipperIdAndDate(userId,date);
            
            result.Payload = _mapper.Map<List<ShippingOrderForShipperResponse>>(shippingOrders);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    // public async Task<OperationResult<List<TotalComboForStaff>>> GetDailyOrderByShipper()
    // {
    //     var result = new OperationResult<List<TotalComboForStaff>>();
    //     var totalFoods = new List<TotalFoodForCompanyResponse>();
    //     var userId = _claimsService.GetCurrentUserId;
    //     try
    //     {
    //         var dailyOrderInclude = new IncludeInfo<ShippingOrder>
    //         {
    //             NavigationProperty = x => x.DailyOrder,
    //             ThenIncludes =
    //             [
    //                 sp => ((DailyOrder)sp).MealSubscription,
    //                 sp => ((MealSubscription)sp).Meal,
    //                 sp => ((MealSubscription)sp).Company,
    //                 sp => ((DailyOrder)sp).Orders,
    //                 sp => ((Order)sp).OrderDetails,
    //                 sp => ((OrderDetail)sp).Combo
    //             ]
    //         };
    //         var shipperInclude = new IncludeInfo<ShippingOrder>
    //         {
    //             NavigationProperty = x => x.Shipper
    //         };
    //         var shippingOrders = await _unitOfWork.ShippingOrderRepository.GetShippingOrderByShipperId(userId);
    //         var comboCounts = new Dictionary<string, int>();
    //         foreach (var shippingOrder in shippingOrders)
    //         {
    //             // Lấy daily order
    //             if (shippingOrder.DailyOrder is null)
    //             {
    //                 result.AddError(ErrorCode.BadRequest, "Company doesn't have daily order");
    //                 return result;
    //             }
    //
    //             // Lấy chi tiết các order detail
    //             var orders = await _unitOfWork.OrderRepository.GetOrderByDailyOrderId(shippingOrder.DailyOrder.Id);
    //             var orderDetails = orders.SelectMany(order => order.OrderDetails).ToList();
    //
    //             // Đếm số lượng từng loại combo và bỏ vào comboCount
    //             foreach (var comboName in orderDetails.Select(orderDetail => orderDetail.Combo.Name))
    //             {
    //                 if (comboCounts.TryGetValue(comboName, out var value))
    //                 {
    //                     comboCounts[comboName] = ++value;
    //                 }
    //                 else
    //                 {
    //                     comboCounts.Add(comboName, 1);
    //                 }
    //             }
    //
    //             // Tạo danh sách totalFoodList từ foodCounts
    //             var totalCombo = comboCounts.Select(kv => new TotalFoodResponse
    //             {
    //                 Name = kv.Key,
    //                 Quantity = kv.Value
    //             }).ToList();
    //             var meal = await _unitOfWork.MealRepository.GetByIdAsync((Guid)shippingOrder.DailyOrder.MealSubscription
    //                 .MealId);
    //             var company =
    //                 await _unitOfWork.CompanyRepository.GetByIdAsync((Guid)shippingOrder.DailyOrder.MealSubscription
    //                     .CompanyId);
    //             var totalFood = new TotalFoodForCompanyResponse()
    //             {
    //                 DailyOrderId = shippingOrder.DailyOrder.Id,
    //                 Meal = meal.MealType,
    //                 DeliveryTime = shippingOrder.DailyOrder.MealSubscription.StartTime?.AddHours(-1),
    //                 CompanyName = company.Name,
    //                 Address = company.Address,
    //                 PhoneNumber = company.PhoneNumber,
    //                 BookingDate = shippingOrder.DailyOrder.BookingDate,
    //                 TotalFoodResponses = totalCombo
    //             };
    //             totalFoods.Add(totalFood);
    //         }
    //
    //         var groupedByDate = totalFoods.GroupBy(f => f.BookingDate)
    //             .Select(g => new TotalComboForStaff(
    //                 BookingDate: g.Key,
    //                 TotalFoodForCompanyResponses: g.ToList()))
    //             .OrderByDescending(g => g.BookingDate)
    //             .ToList();
    //
    //         result.Payload = groupedByDate;
    //     }
    //     catch (Exception e)
    //     {
    //         result.AddUnknownError(e.Message);
    //     }
    //
    //     return result;
    // }

    public async Task<OperationResult<DailyOrderResponse>> ConfirmShippingOrderByShipper(Guid dailyOrderId)
    {
        var result = new OperationResult<DailyOrderResponse>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var shippingOrders = await _unitOfWork.ShippingOrderRepository
                .GetShippingOrderByDailyOrderId(dailyOrderId);
            // check đơn hàng có phải phân công đúng của shipper không ?
            if (!shippingOrders.Any() || shippingOrders.All(order => order.ShipperId != userId))
            {
                result.AddError(ErrorCode.BadRequest, "Đơn hàng này không phải phân công của bạn");
                return result;
            }
            
            // check xem xem tổng đơn(dailyOrder) đã ở trạng thái sẵn sàng chờ đến lấy đi giao hay chưa ? 
            var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync(dailyOrderId);
            if (dailyOrder is not { Status: DailyOrderStatus.Waiting })
            {
                switch (dailyOrder.Status)
                {
                    case DailyOrderStatus.Complete:
                        result.AddError(ErrorCode.BadRequest, "Đơn đã hoàn thành rồi nhé");
                        return result;
                    case DailyOrderStatus.Cooking:
                        result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá trình nấu");
                        return result;
                    case DailyOrderStatus.Initial:
                        result.AddError(ErrorCode.BadRequest, "Đơn chưa sẵn sàng");
                        return result;
                    case DailyOrderStatus.Processing:
                        result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá xử lý");
                        return result;
                    case DailyOrderStatus.Waiting:
                        result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá trình nấu");
                        return result;
                    case DailyOrderStatus.Delivering:
                        result.AddError(ErrorCode.BadRequest, "Đơn đã đang trong quá trình giao");
                        return result;
                    default:
                        result.AddError(ErrorCode.BadRequest, "Không thể xác nhận đơn hàng lúc này");
                        return result;
                }
            }
            
            if (shippingOrders.All(s => s.Status == ShippingStatus.Pending))
            {
                foreach (var shippingOrder in shippingOrders)
                {
                    shippingOrder.Status = ShippingStatus.Confirm;
                } 
            }
            else
            {
                result.AddError(ErrorCode.ServerError, "Có 1 đơn vận chuyển không phải trong trạng thái chờ xác nhận");
                return result;
            }
            _unitOfWork.ShippingOrderRepository.UpdateRange(shippingOrders);
            dailyOrder.Status = DailyOrderStatus.Delivering;
            _unitOfWork.DailyOrderRepository.Update(dailyOrder);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "lỗi trong quá trình lưu xuống db");
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

    public async Task<OperationResult<List<TotalFoodForCompanyResponse>>> GetHistoryDailyOrderByShipper()
    {
        var result = new OperationResult<List<TotalFoodForCompanyResponse>>();
        var totalFoods = new List<TotalFoodForCompanyResponse>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var shippingOrders = await _unitOfWork.ShippingOrderRepository.GetShippingOrderByShipperId(userId);
            var comboCounts = new Dictionary<string, int>();
            foreach (var shippingOrder in shippingOrders)
            {
                // Lấy daily order
                var dailyOrder = await _unitOfWork.DailyOrderRepository.GetById((Guid)shippingOrder.DailyOrderId);
                if (dailyOrder is null)
                {
                    result.AddError(ErrorCode.BadRequest, "Company doesn't have daily order");
                    return result;
                }

                // Lấy chi tiết các order detail
                var orders = await _unitOfWork.OrderRepository.GetOrderByDailyOrderId(dailyOrder.Id);
                var orderDetails = orders.SelectMany(order => order.OrderDetails).ToList();

                // Đếm số lượng từng loại combo và bỏ vào comboCount
                foreach (var orderDetail in orderDetails)
                {
                    // Nếu là combo thì lấy chi tiết các food trong combo
                    var combo = await _unitOfWork.ComboRepository.GetByIdAsync(orderDetail.Combo.Id);
                    var comboName = orderDetail.Combo.Name;

                    if (comboCounts.ContainsKey(comboName))
                    {
                        comboCounts[comboName]++;
                    }
                    else
                    {
                        comboCounts.Add(comboName, 1);
                    }
                }

                // Tạo danh sách totalFoodList từ foodCounts
                var totalCombo = comboCounts.Select(kv => new TotalFoodResponse
                {
                    Name = kv.Key,
                    Quantity = kv.Value
                }).ToList();
                var meal = await _unitOfWork.MealRepository.GetByIdAsync((Guid)dailyOrder.MealSubscription.MealId);
                var com = await _unitOfWork.CompanyRepository.GetByIdAsync((Guid)dailyOrder.MealSubscription.CompanyId);
                var totalFood = new TotalFoodForCompanyResponse()
                {
                    DailyOrderId = dailyOrder.Id,
                    Meal = meal.MealType,
                    DeliveryTime = dailyOrder.MealSubscription.StartTime?.AddHours(-1),
                    CompanyName = com.Name,
                    Address = com.Address,
                    PhoneNumber = com.PhoneNumber,
                    BookingDate = dailyOrder.BookingDate,
                    TotalFoodResponses = totalCombo
                };
                totalFoods.Add(totalFood);
            }

            result.Payload = totalFoods;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<UserResponse>>> GetSDeliveryStaffByDailyOrder(Guid dailyOrderId)
    {
        var result = new OperationResult<List<UserResponse>>();
        try
        {
            var shippingOrders = await _unitOfWork.ShippingOrderRepository.GetShippingOrderByDailyOrder(dailyOrderId);
            if (shippingOrders != null)
            {
                var users = shippingOrders
                    .Select(user =>
                    {
                        if (user.Shipper != null)
                            return new UserResponse
                            {
                                Id = user.Shipper.Id,
                                Email = user.Shipper.Email,
                                Name = user.Shipper.Name,
                                Image = user.Shipper.Image,
                                Code = user.Shipper.Code.ToString(),
                                PhoneNumber = user.Shipper.PhoneNumber,
                                EmailConfirmed = user.Shipper.EmailConfirmed,
                                LockoutEnabled = user.Shipper.LockoutEnabled
                            };
                        return null;
                    }).ToList();
                result.Payload = users;
            }
            else
            {
                result.AddError(ErrorCode.BadRequest, "Đơn hàng chưa được phân công");
            }
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}