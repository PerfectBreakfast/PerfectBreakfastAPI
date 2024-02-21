using System.Linq.Expressions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly IPayOsService _payOsService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, IPayOsService payOsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
            _payOsService = payOsService;
        }

        public async Task<OperationResult<PaymentResponse>> CreateOrder(OrderRequest orderRequest)
        {
            var userId = _claimsService.GetCurrentUserId;
            var result = new OperationResult<PaymentResponse>();
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
                    var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(od.ComboId);
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
                int orderCode = Utils.RandomCode.GenerateOrderCode();
                order.OrderCode = orderCode;
                order.WorkerId = userId;
                order.OrderStatus = OrderStatus.Pending;
                order.DailyOrder = dailyOrder;
                order.TotalPrice = totalPrice;
                order.OrderDetails = orderDetail;
                var entity = await _unitOfWork.OrderRepository.AddAsync(order);

                var paymentMethod = orderRequest.Payment.ToUpper();
                switch (paymentMethod)
                {
                    case "BANKING":        // Gọi phương thức tạo paymentLink Ngân hàng 
                        /*var paymentResponse = await _payOsService.CreatePaymentLink(entity);
                        if (paymentResponse.IsSuccess)
                        {
                            result.Payload = paymentResponse;
                        }*/
                        
                        // không chơi tạo link thanh toán nữa test cho dễ
                        result.Payload = new PaymentResponse
                        {
                            IsSuccess = true,
                            DeepLink = null,
                            PaymentUrl = "thành công rồi mà không trả link",
                            QrCode = "QRcode"
                        };
                        entity.OrderStatus = OrderStatus.Paid;
                        /*else
                        {
                            throw new Exception("xảy ra lỗi khi tạo link thanh toán Ngân hàng");
                        }*/
                        break;

                    case "MOMO":          // Gọi tạo PaymentLink MoMO
                        break;
                }
                await _unitOfWork.SaveChangeAsync();
            }
            catch (NotFoundIdException e)
            {
                result.AddUnknownError("UserId is not exsit");
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
                result.AddUnknownError("Id is not exsit");
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
                //var orderDetails = _mapper.Map<List<OrderDetailResponse>>(order.OrderDetails);
                var orderDetails = new List<OrderDetailResponse>();
                foreach(var detail in order.OrderDetails)
                {
                    var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(detail.ComboId);
                    var foodEntities = combo.ComboFoods.Select(cf => cf.Food).ToList();
                    var orderDetailResponse = new OrderDetailResponse
                    {
                        ComboName = combo.Name,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        Image = combo.Image,
                        Foods = $"{string.Join(", ", foodEntities.Select(food => food.Name))}"
                    };

                    orderDetails.Add(orderDetailResponse);
                }

                var or = _mapper.Map<OrderResponse>(order);
                or.orderDetails = orderDetails;
                result.Payload = or;
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<Pagination<OrderResponse>>> GetOrderPaginationAsync(int pageIndex = 0, int pageSize = 10)
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

        public async Task<OperationResult<List<OrderHistoryResponse>>> GetOrderHistory()
        {
            var result = new OperationResult<List<OrderHistoryResponse>>();
            var userId = _claimsService.GetCurrentUserId;
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, x => x.Company);
                var orderdetailInclude = new IncludeInfo<Order>
                {
                    NavigationProperty = x => x.OrderDetails,
                    ThenIncludes = new List<Expression<Func<object, object>>>
                    {
                        sp => ((OrderDetail)sp).Combo
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
                var orders = await _unitOfWork.OrderRepository.GetOrderHistory(userId,orderdetailInclude,workerInclude);
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
