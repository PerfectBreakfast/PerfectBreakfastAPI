using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
        }

        public async Task<OperationResult<OrderResponse>> CreateOrder(OrderRequest orderRequest)
        {
            var userId = _claimsService.GetCurrentUserId;
            var result = new OperationResult<OrderResponse>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                var dailyOrder = await _unitOfWork.DailyOrderRepository.FindByCompanyId((Guid)user.CompanyId);
                if (dailyOrder is null)
                {
                    result.AddUnknownError("CompanyId is not exsit");
                    return result;
                }
                var order = _mapper.Map<Order>(orderRequest);
                var orderDetail = _mapper.Map<List<OrderDetail>>(orderRequest.OrderDetails);
                foreach (var od in orderDetail)
                {
                    // Fetch Combo by Id
                    var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync((Guid)od.ComboId);
                    if (combo is null)
                    {
                        result.AddUnknownError("ComboId is not exsit");
                        return result;
                    }
                    if (combo != null)
                    {
                        var foods = combo.ComboFoods.Select(cf => cf.Food).ToList();
                        if (foods is null)
                        {
                            result.AddUnknownError("Combo khong co thuc an");
                            return result;
                        }
                        decimal totalFoodPrice = foods.Sum(food => food.Price);
                        od.UnitPrice = totalFoodPrice;
                    }
                    else
                    {
                        result.AddUnknownError($"Combo with Id {od.ComboId} not found.");
                        return result;
                    }
                }
                decimal totalPrice = orderDetail.Sum(detail => detail.Quantity * detail.UnitPrice);
                int orderCode = Utils.Random.GenerateCode();
                order.OrderCode = orderCode;
                order.WorkerId = userId;
                order.OrderStatus = OrderStatus.Pending;
                order.DailyOrder = dailyOrder;
                order.TotalPrice = totalPrice;
                order.OrderDetails = orderDetail;
                await _unitOfWork.OrderRepository.AddAsync(order);
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<OrderResponse>(order);
            }
            catch (NotFoundIdException)
            {
                result.AddUnknownError("UserId is not exsit");
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
                var order = await _unitOfWork.OrderRepository.FindSingleAsync(o => o.Id == id, or => or.OrderDetails);
                if (order is null)
                {
                    result.AddUnknownError("Id is not exsit");
                    return result;
                }
                result.Payload = _mapper.Map<OrderResponse>(order);
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


    }
}
