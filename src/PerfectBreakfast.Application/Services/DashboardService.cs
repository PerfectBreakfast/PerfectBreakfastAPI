using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;
    private readonly ICurrentTime _currentTime;

    public DashboardService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, ICurrentTime currentTime)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        _currentTime = currentTime;
        
    }
    public async Task<OperationResult<List<CompanyStatistic>>> GetTop10CompanyOrder(DateOnly? fromDate, DateOnly? toDate)
    {
        var result = new OperationResult<List<CompanyStatistic>>();
        try
        {
            var today = _currentTime.GetCurrentTime();
            fromDate ??= DateOnly.FromDateTime(today).AddDays(-7);
            toDate ??= DateOnly.FromDateTime(today);
            
            if (fromDate >= toDate)
            {
                result.AddError(ErrorCode.BadRequest, "'Từ ngày' phải nhỏ hơn với 'đến ngày'");
            }
            var totalOrders = await _unitOfWork.OrderRepository.GetOrderByDateForCompany(fromDate, toDate);
            var totalQuantityByDate = totalOrders
                .GroupBy(order => (order.DailyOrder.MealSubscription.Company.Id, 
                    order.DailyOrder.MealSubscription.Company.Name,
                    order.DailyOrder.MealSubscription.Company.PhoneNumber,
                    order.DailyOrder.MealSubscription.Company.Email,
                    order.DailyOrder.MealSubscription.Company.Address))
                .Select(group => new CompanyStatistic
                (
                    Id: group.Key.Id,
                    Name:group.Key.Name,
                    PhoneNumber: group.Key.PhoneNumber,
                    Email: group.Key.Email,
                    Address: group.Key.Address,
                    OrderQuantity: group.Count() 
                ))
                .OrderByDescending(stat => stat.OrderQuantity)
                .Take(10)
                .ToList();
            result.Payload = totalQuantityByDate;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<TopComboAndFoodResponse>>> GetTopComboAndFood(DateOnly? fromDate, DateOnly? toDate)
    {
        var result = new OperationResult<List<TopComboAndFoodResponse>>();
        try
        {
            var today = _currentTime.GetCurrentTime();
            fromDate ??= DateOnly.FromDateTime(today).AddDays(-7);
            toDate ??= DateOnly.FromDateTime(today);
            
            if (fromDate >= toDate)
            {
                result.AddError(ErrorCode.BadRequest, "'Từ ngày' phải nhỏ hơn với 'đến ngày'");
            }
            
            var totalOrders = await _unitOfWork.OrderRepository.GetOrderByDateForComboAndFood(fromDate, toDate);
            var topItems = totalOrders
                .SelectMany(order => order.OrderDetails) // Lấy tất cả OrderDetails từ tất cả đơn hàng
                // Biến đổi mỗi OrderDetail thành đối tượng có Id, Type, Quantity, Image và Price
                .Select(detail => new
                {
                    Id = detail.Combo != null ? detail.Combo.Id : detail.Food.Id,
                    Type = detail.Combo != null ? "Combo" : "Đơn lẻ",
                    Name = detail.Combo != null ? detail.Combo.Name : detail.Food.Name,
                    Quantity = detail.Quantity,
                    Image = detail.Combo != null ? detail.Combo.Image : detail.Food.Image,
                    Price = detail.Combo != null ? detail.Combo.ComboFoods.Sum(cf => cf.Food.Price) : detail.Food.Price,
                    Foods = detail.Combo != null ? string.Join(", ", detail.Combo.ComboFoods.Select(cf => cf.Food.Name)) : null
                })
                .GroupBy(item => new { item.Id, item.Type }) // Nhóm theo Id và loại (Combo/Food)
                .Select(group => new TopComboAndFoodResponse
                (
                    Id: group.Key.Id,
                    Type: group.Key.Type,
                    Name: group.First().Name,
                    Quantity: group.Sum(item => item.Quantity), // Tính tổng số lượng cho mỗi nhóm
                    Image: group.First().Image, // Lấy hình ảnh từ một mục đại diện trong nhóm
                    Price: group.First().Price, // Lấy giá từ một mục đại diện trong nhóm
                    Foods: group.First().Foods ?? "" 
                ))
                .OrderByDescending(item => item.Quantity)
                .Take(10) // Lấy top 10
                .ToList();
            result.Payload = topItems;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<OrderStatisticResponse>> OrderStatistic(DateOnly? fromDate, DateOnly? toDate)
    {
        var result = new OperationResult<OrderStatisticResponse>();
        try
        {
            var today = _currentTime.GetCurrentTime();
            fromDate ??= DateOnly.FromDateTime(today).AddDays(-7);
            toDate ??= DateOnly.FromDateTime(today);
            
            if (fromDate >= toDate)
            {
                result.AddError(ErrorCode.BadRequest, "'Từ ngày' phải nhỏ hơn với 'đến ngày'");
            }
            
            var totalOrders = await _unitOfWork.OrderRepository.GetOrderByDate(fromDate, toDate);
            var completeOrders = totalOrders.Where(o => o.OrderStatus == OrderStatus.Complete).ToList();
            var cancelledForOverdueOrders = totalOrders.Where(o => o.OrderStatus == OrderStatus.CancelledForOverdue).ToList();
            var completePercent = ((double)completeOrders.Count / totalOrders.Count) * 100;
            var cancelledForOverduePercent = ((double)cancelledForOverdueOrders.Count / totalOrders.Count) * 100;
            var orderStatistic = new OrderStatisticResponse(fromDate, toDate, totalOrders.Count, completeOrders.Count, Math.Round(completePercent, 2), cancelledForOverdueOrders.Count, Math.Round(cancelledForOverduePercent, 2));
            result.Payload = orderStatistic;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<OrderByDateStatistic>>> OrderAmountByDateStatistic(DateOnly? fromDate, DateOnly? toDate)
    {
        var result = new OperationResult<List<OrderByDateStatistic>>();
        try
        {
            var today = _currentTime.GetCurrentTime();
            fromDate ??= DateOnly.FromDateTime(today).AddDays(-7);
            toDate ??= DateOnly.FromDateTime(today);
            
            if (fromDate >= toDate)
            {
                result.AddError(ErrorCode.BadRequest, "'Từ ngày' phải nhỏ hơn với 'đến ngày'");
            }
            
            var totalOrders = await _unitOfWork.OrderRepository.GetOrderByDate(fromDate, toDate);
            var totalQuantityByDate = totalOrders
                .GroupBy(order => order.CreationDate.Date) // Nhóm theo ngày tạo.
                .Select(group => new OrderByDateStatistic
                (
                    Date: DateOnly.FromDateTime(group.Key),
                    OrderAmount: group.Count() 
                ))
                .ToList();
            result.Payload = totalQuantityByDate;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}