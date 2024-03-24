using System.Linq.Expressions;
using System.Text.Json;
using Hangfire;
using Hangfire.Server;
using MapsterMapper;
using Microsoft.Extensions.Caching.Distributed;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;
    private readonly IPayOsService _payOsService;
    private readonly IDistributedCache _cache;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService,
        IPayOsService payOsService,IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        _payOsService = payOsService;
        _cache = cache;
    }

    public async Task<OperationResult<PaymentResponse>> CreateOrder(OrderRequest orderRequest)
    {
        var userId = _claimsService.GetCurrentUserId;
        var paymentResponse = new PaymentResponse();
        var result = new OperationResult<PaymentResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            
            // fix lại đoạn bốc DailyOrder này cho nó sờ mút ==================
            // ============================================================================
            var mealSubscription = await _unitOfWork.MealSubscriptionRepository.FindByCompanyId((Guid)user.CompanyId, orderRequest.MealId);
            if (mealSubscription == null)
            {
                result.AddError(ErrorCode.BadRequest, "Company does not have this meal");
                return result;
            }
            var dailyOrder = await _unitOfWork.DailyOrderRepository.FindByMealSubscription(mealSubscription.Id);
            if (dailyOrder is null)
            {
                result.AddError(ErrorCode.NotFound, "Meal is not exist");
                return result;
            }
            var order = _mapper.Map<Order>(orderRequest);
            order.MealId = orderRequest.MealId;
            var orderDetails = _mapper.Map<List<OrderDetail>>(orderRequest.OrderDetails);
            foreach (var od in orderDetails)
            {
                if (od.ComboId != null)
                {
                    var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(od.ComboId);
                    if (combo == null)
                    {
                        result.AddUnknownError($"Combo with Id {od.ComboId} not found.");
                        return result;
                    }
                    var foods = combo.ComboFoods.Select(cf => cf.Food).ToList();
                    if (foods == null)
                    {
                        result.AddError(ErrorCode.BadRequest, "Combo khong co thuc an");
                        return result;
                    }
                    
                    decimal totalFoodPrice = foods.Sum(food => food.Price);
                    od.UnitPrice = totalFoodPrice;
                }
                else
                {
                    var food = await _unitOfWork.FoodRepository.GetByIdAsync((Guid)od.FoodId!);
                    if (food.FoodStatus != FoodStatus.Retail)
                    {
                        result.AddUnknownError($"Món này không hợp lệ");
                        return result;
                    }
                    od.UnitPrice = food.Price;
                }
                
            }
            // lấy phương thức thanh toán
            var paymentMethod = orderRequest.Payment.ToUpper();
            
            var totalPrice = orderDetails.Sum(detail => detail.Quantity * detail.UnitPrice);
            var orderCode = Utils.RandomCode.GenerateOrderCode();
            order.OrderCode = orderCode;
            order.WorkerId = userId;
            order.OrderStatus = OrderStatus.Pending;
            order.DailyOrder = dailyOrder;
            order.TotalPrice = totalPrice;
            order.OrderDetails = orderDetails!;
            order.PaymentMethodId = (await _unitOfWork.PaymentMethodRepository.FindSingleAsync(x => x.Name == paymentMethod)).Id;
            var entity = await _unitOfWork.OrderRepository.AddAsync(order);
            
            switch (paymentMethod)
            {
                case ConstantPaymentMethod.BANKING: 
                    // Gọi phương thức tạo paymentLink Ngân hàng 
                    paymentResponse = await _payOsService.CreatePaymentLink(entity);
                    if (paymentResponse.IsSuccess)
                    {
                        result.Payload = paymentResponse;
                    }

                    // không chơi tạo link thanh toán nữa test cho dễ
                    /*result.Payload = new PaymentResponse
                    {
                        IsSuccess = true,
                        DeepLink = null,
                        PaymentUrl = "thành công rồi mà không trả link",
                        QrCode = "QRcode"
                    };
                    entity.OrderStatus = OrderStatus.Paid;*/

                     else
                     {
                         throw new Exception("xảy ra lỗi khi tạo link thanh toán Ngân hàng");
                     }
                    break;

                case ConstantPaymentMethod.MOMO:
                    // Gọi tạo PaymentLink MoMO
                    break;
            }
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Food or Combo is not exist");
                return result;
            }

            // chỗ này lưu đối tượng thanh toán (paymentLink, deepLink....)
            await _cache.SetAsync($"order-{entity.Id}", JsonSerializer.SerializeToUtf8Bytes(paymentResponse),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ConstantTime.ORDER_CACHE_DURATION_MINUTES),  // set Thời gian cache tồn tại
                });

            // tạo job check sau 15p chưa thanh toán thì sẽ cancel order
            var timeToCancel = DateTime.UtcNow.AddMinutes(ConstantTime.JOB_DELAY_BEFORE_START_MINUTES);
            string id = BackgroundJob.Schedule<IManagementService>(
                x => x.AutoCancelOrderWhenOverTime(entity.Id), timeToCancel);
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound, "UserId is not exist");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<PaymentResponse>> GetLinkContinuePayment(Guid id)
    {
        var result = new OperationResult<PaymentResponse>();
        try
        {
            var paymentResponse = await _cache.GetAsync($"order-{id}");
            if (paymentResponse is null)
            {
                result.AddError(ErrorCode.NotFound, "Không tồn tại phương phức thanh toán");
                return result;
            }
            result.Payload = JsonSerializer.Deserialize<PaymentResponse>(paymentResponse);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<PaymentResponse>> CancelOrder(long orderCode)
    {
        var result = new OperationResult<PaymentResponse>();
        try
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByOrderCode(orderCode);
            if (order.OrderStatus != OrderStatus.Pending)
            {
                result.AddError(ErrorCode.BadRequest,"Không thể hủy vì đã thanh toán hoặc hoàn thành");
                return result;
            }
            order.OrderStatus = OrderStatus.Cancel;
            _unitOfWork.OrderRepository.Update(order);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError,"Lưu xuống db có vấn đề");
                return result;
            }
            // xóa cache
            await _cache.RemoveAsync($"order-{order.Id}");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<OrderResponse>> DeleteOrder(Guid id)
    {
        var result = new OperationResult<OrderResponse>();
        try
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            _unitOfWork.OrderRepository.SoftRemove(order);
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

    public async Task<OperationResult<OrderResponse>> GetOrder(Guid id)
    {
        var result = new OperationResult<OrderResponse>();
        try
        {
            var userInclude = new IncludeInfo<Order>
            {
                NavigationProperty = c => c.Worker,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((User)sp).Company
                }
            };
            var dailyOrderInclude = new IncludeInfo<Order>
            {
                NavigationProperty = c => c.DailyOrder,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((DailyOrder)sp).MealSubscription,
                        sp => ((MealSubscription)sp).Meal,
                }
            };
            var paymentMethodInclude = new IncludeInfo<Order>
            {
                NavigationProperty = c => c.PaymentMethod
            };
            var comboInclude = new IncludeInfo<Order>
            {
                NavigationProperty = c => c.OrderDetails,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((OrderDetail)sp).Combo,
                        sp => ((Combo)sp).ComboFoods,
                            sp => ((ComboFood)sp).Food,
                }
            };
            var foodInclude = new IncludeInfo<Order>
            {
                NavigationProperty = c => c.OrderDetails,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((OrderDetail)sp).Food
                }
            };
            
            var order = await _unitOfWork.OrderRepository.GetByIdAndIncludeAsync(id, userInclude, dailyOrderInclude,
                paymentMethodInclude, comboInclude, foodInclude);

            // lấy thông tin công ty của worker
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(order.Worker.CompanyId.Value);

            var orderDetails = new List<OrderDetailResponse>();
            foreach (var detail in order.OrderDetails)
            {
                if (detail.Combo != null)
                {
                    var foodEntities = detail.Combo.ComboFoods.Select(cf => cf.Food).ToList();
                    var orderDetailResponse = new OrderDetailResponse
                    {
                        ComboName =  detail.Combo.Name,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        Image =  detail.Combo.Image,
                        Foods = $"{string.Join(", ", foodEntities.Select(food => food.Name))}"
                    };
                    orderDetails.Add(orderDetailResponse);
                }
                else if(detail.Food != null)
                {
                    var orderDetailResponse = new OrderDetailResponse
                    {
                        ComboName = "",
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        Image = detail.Food.Image,
                        Foods = detail.Food.Name
                    };
                    orderDetails.Add(orderDetailResponse);
                }
            }

            var or = _mapper.Map<OrderResponse>(order);
            or.PaymentMethod = order.PaymentMethod?.Name ?? null;
            or.BookingDate = order.DailyOrder!.BookingDate;
            or.Company = _mapper.Map<CompanyDto>(company);
            or.OrderDetails = orderDetails;
            or.User = _mapper.Map<UserResponse>(order.Worker);
            result.Payload = or;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Pagination<OrderResponse>>> GetOrderPaginationAsync(int pageIndex = 0,
        int pageSize = 10)
    {
        var result = new OperationResult<Pagination<OrderResponse>>();
        try
        {
            var order = await _unitOfWork.OrderRepository.ToPagination(pageIndex, pageSize);
            result.Payload = _mapper.Map<Pagination<OrderResponse>>(order);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<OrderHistoryResponse>>> GetOrderHistory(int pageNumber = 1)
    {
        pageNumber *= 5;
        var result = new OperationResult<List<OrderHistoryResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            //var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            var orderDetailInclude = new IncludeInfo<Order>
            {
                NavigationProperty = x => x.OrderDetails,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((OrderDetail)sp).Combo
                }
            };
            var dailyOrderInclude = new IncludeInfo<Order>
            {
                NavigationProperty = x => x.DailyOrder,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((DailyOrder)sp).MealSubscription,
                    sp => ((MealSubscription)sp).Meal
                }
            };
            var workerInclude = new IncludeInfo<Order>
            {
                NavigationProperty = x => x.Worker,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((User)sp).Company
                }
            };
            var orders = await _unitOfWork.OrderRepository.GetOrderHistory(userId, pageNumber, orderDetailInclude, dailyOrderInclude, workerInclude);
            result.Payload = _mapper.Map<List<OrderHistoryResponse>>(orders);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<OrderResponse>> RemoveOrder(Guid id)
    {
        var result = new OperationResult<OrderResponse>();
        try
        {
            // find supplier by ID
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            // Remove
            var entity = _unitOfWork.OrderRepository.Remove(order);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<OrderResponse>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> CompleteOrder(Guid id)
    {
        var result = new OperationResult<bool>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            // find supplier by ID
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id, x => x.DailyOrder!);
            
            // check nếu order này chưa thanh toán hoặc cái dailyOrder của nó đang không ở trạng thái Delivering
            if (order.OrderStatus != OrderStatus.Paid || order.DailyOrder!.Status != DailyOrderStatus.Delivering)
            {
                result.AddError(ErrorCode.BadRequest, $"Thời gian giao hàng không phù hợp! {order.DailyOrder!.Status.ToString()}");
                return result;
            }
            
            // kiểm tra xem thg quét có được giao cho cái dailyOrder
            var shippingOrder = await _unitOfWork.ShippingOrderRepository.GetShippingOrderByDailyOrderId(userId,
                    order.DailyOrder!.Id);
            if (shippingOrder is null)
            {
                result.AddError(ErrorCode.BadRequest, "Bạn không được giao đơn hàng này!");
                return result;
            }
            
            // Change Status
            order.OrderStatus = OrderStatus.Complete;
            // add id của thằng đã giao cái đơn đó
            order.DeliveryStaffId = userId;
            // update
            _unitOfWork.OrderRepository.Update(order);
            // saveChange
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            // map
            result.Payload = isSuccess;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<OrderStatisticResponse>> OrderStatistic(DateOnly fromDate, DateOnly toDate)
    {
        var result = new OperationResult<OrderStatisticResponse>();
        try
        {
            if (fromDate >= toDate || toDate.AddDays(-2) <= fromDate)
            {
                result.AddError(ErrorCode.BadRequest, "'Từ ngày' phải nhỏ hơn ít nhất 2 ngày so với 'đến ngày'");
            }
            var totalOrders = await _unitOfWork.OrderRepository.GetOrderByDate(fromDate, toDate);
            var completeOrders = totalOrders.Where(o => o.OrderStatus == OrderStatus.Complete).ToList();

            // Tính toán số lần xuất hiện của mỗi ComboId trong tất cả các OrderDetail
            var comboIdOccurrences = completeOrders
                .SelectMany(order => order.OrderDetails) // Lấy tất cả các OrderDetail từ mỗi CompleteOrder
                .GroupBy(detail => detail.ComboId) // Nhóm theo ComboId
                .Select(group => new
                {
                    ComboId = group.Key,
                    Count = group.Count() // Đếm số lần xuất hiện của mỗi ComboId
                })
                .OrderByDescending(combo => combo.Count) // Sắp xếp giảm dần theo số lần xuất hiện
                .FirstOrDefault(); // Lấy ComboId xuất hiện nhiều nhất

            // Trả về ComboId phổ biến nhất, hoặc null nếu không tìm thấy
            var comboPopularId = comboIdOccurrences?.ComboId;

            //Lấy food name trong combo
            var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(comboPopularId);
            var comboPopular = "Không có món hoàn thành";
            if (combo != null)
            {
                var foodEntities = combo.ComboFoods.Select(cf => cf.Food).ToList();
                comboPopular = $"{string.Join(", ", foodEntities.Select(food => food.Name))}";
            }
            var orderStatistic = new OrderStatisticResponse(fromDate, toDate, totalOrders.Count, completeOrders.Count, comboPopular);
            result.Payload = orderStatistic;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<OrderResponse>>> GetOrderByDailyOrder(Guid dailyOrderId)
    {
        var result = new OperationResult<List<OrderResponse>>();
        try
        {
            var orderInclude = new IncludeInfo<DailyOrder>
            {
                NavigationProperty = x => x.Orders,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Order)sp).PaymentMethod
                }
            };
            var workerInclude = new IncludeInfo<DailyOrder>
            {
                NavigationProperty = x => x.Orders,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Order)sp).Worker
                }
            };
            var comboInclude = new IncludeInfo<DailyOrder>
            {
                NavigationProperty = x => x.Orders,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Order)sp).OrderDetails,
                        sp => ((OrderDetail)sp).Combo
                }
            };
            var foodInclude = new IncludeInfo<DailyOrder>
            {
                NavigationProperty = x => x.Orders,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Order)sp).OrderDetails,
                        sp => ((OrderDetail)sp).Food
                }
            };
            var dailyOrder = await _unitOfWork.DailyOrderRepository
                .GetByIdAndIncludeAsync(dailyOrderId, orderInclude, workerInclude, comboInclude, foodInclude);
            result.Payload = _mapper.Map<List<OrderResponse>>(dailyOrder.Orders);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<OrderResponse>>> GetOrders()
    {
        var result = new OperationResult<List<OrderResponse>>();
        try
        {
            var order = await _unitOfWork.OrderRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<OrderResponse>>(order);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<OrderResponse>> UpdateOrder(Guid id, UpdateOrderRequest updateOrderRequest)
    {
        var result = new OperationResult<OrderResponse>();
        try
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            _mapper.Map(updateOrderRequest, order);
            _unitOfWork.OrderRepository.Update(order);
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