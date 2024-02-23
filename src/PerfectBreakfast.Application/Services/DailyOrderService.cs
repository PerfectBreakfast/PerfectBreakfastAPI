using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Request;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PerfectBreakfast.Application.Services
{
    public class DailyOrderService : IDailyOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentTime _currentTime;
        private readonly IClaimsService _claimsService;

        public DailyOrderService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentTime = currentTime;
            _claimsService = claimsService;
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
                        sp => ((Partner)sp).Companies
                    }
                };
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);
                // get Companies of Delivery
                var companyIds = user.Partner.Companies.Select(u => u.Id).ToList();

                // Xây dựng predicate để lọc DailyOrder theo các CompanyId
                Expression<Func<DailyOrder, bool>> predicate = order => companyIds.Contains(order.CompanyId.Value);
                // Get Paging
                var companyInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.Company
                };
                var dailyOrderPages = await _unitOfWork.DailyOrderRepository.ToPagination(pageIndex, pageSize,predicate,companyInclude);
                // Group by Booking Order
                var dailyOrderByBookingOrder = dailyOrderPages.Items.GroupBy(x => x.BookingDate)
                    .ToDictionary(x => x.Key, g => g.ToList());
                // custom output
                var dailyOrderResponse = dailyOrderByBookingOrder.Select(x =>
                    new DailyOrderForPartnerResponse(
                        x.Key, // BookingDate từ Dictionary
                        _mapper.Map<List<DailyOrderModelResponse>>(x.Value)
                    )).ToList();
                
                result.Payload = new Pagination<DailyOrderForPartnerResponse>
                {
                    PageIndex = dailyOrderPages.PageIndex,
                    PageSize = dailyOrderPages.PageSize,
                    TotalItemsCount = dailyOrderPages.TotalItemsCount,
                    Items = dailyOrderResponse
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
                        sp => ((Delivery)sp).Companies
                    }
                };
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, deliveryInclude);
                // check xem user đó có phải là trong đơn vị không và role của user có phải là Admin hay không 
                
                
                // get Companies of Delivery
                var companyIds = user.Delivery.Companies.Select(u => u.Id).ToList();
                // Xây dựng predicate để lọc DailyOrder theo các CompanyId
                Expression<Func<DailyOrder, bool>> predicate = order => companyIds.Contains(order.CompanyId.Value);
                // Get Paging
                var companyInclude = new IncludeInfo<DailyOrder>
                {
                    NavigationProperty = x => x.Company
                };
                var dailyOrderPages = await _unitOfWork.DailyOrderRepository.ToPagination(pageIndex, pageSize,predicate,companyInclude);
                // Group by Booking Order
                var dailyOrderByBookingOrder = dailyOrderPages.Items.GroupBy(x => x.BookingDate)
                    .ToDictionary(x => x.Key, g => g.ToList());
                // custom output
                var dailyOrderResponse = dailyOrderByBookingOrder.Select(x =>
                    new DailyOrderForDeliveryResponse(
                        x.Key, // BookingDate từ Dictionary
                        _mapper.Map<List<DailyOrderModelResponse>>(x.Value)
                    )).ToList();
                
                result.Payload = new Pagination<DailyOrderForDeliveryResponse>
                {
                    PageIndex = dailyOrderPages.PageIndex,
                    PageSize = dailyOrderPages.PageSize,
                    TotalItemsCount = dailyOrderPages.TotalItemsCount,
                    Items = dailyOrderResponse
                };
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }


        public async Task<OperationResult<TotalFoodForCompanyResponse>> GetDailyOrderDetailByPartner(Guid id, DateOnly bookingDate)
        {
            var result = new OperationResult<TotalFoodForCompanyResponse>();
            try
            {
                var company = await _unitOfWork.CompanyRepository.GetCompanyById(id);
                if (company == null)
                {
                    result.AddUnknownError("Company is not exsit");
                    return result;
                }
                var foodCounts = new Dictionary<string, int>();

                // Lấy daily order
                var dailyOrder = company.DailyOrders.SingleOrDefault(x => x.BookingDate == bookingDate);

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
                            //... cộng dồn số lượng cho từng food
                            if (foodCounts.ContainsKey(foodName))
                            {
                                foodCounts[foodName] += orderDetail.Quantity;
                            }
                            else
                            {
                                foodCounts[foodName] = orderDetail.Quantity;
                            }
                        }

                    }
                    else if (orderDetail.Food != null)
                    {
                        // Xử lý order detail là food đơn lẻ
                        var foodName = orderDetail.Food.Name;
                        // cộng dồn số lượng cho từng food
                        if (foodCounts.ContainsKey(foodName))
                        {
                            foodCounts[foodName] += orderDetail.Quantity;
                        }
                        else
                        {
                            foodCounts[foodName] = orderDetail.Quantity;
                        }
                    }
                }
                // Tạo danh sách totalFoodList từ foodCounts
                var totalFoodList = foodCounts.Select(pair => new TotalFoodResponse { Name = pair.Key, Quantity = pair.Value }).ToList();
                var totalFoodForCompany = new TotalFoodForCompanyResponse()
                {
                    CompanyName = company.Name,
                    PhoneNumber = company.PhoneNumber,
                    BookingDate = dailyOrder.BookingDate,
                    Status = dailyOrder.Status.ToString(),
                    TotalFoodResponses = totalFoodList
                };
                result.Payload = totalFoodForCompany;
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

        public async Task<OperationResult<Pagination<DailyOrderResponse>>> GetDailyOrderPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<DailyOrderResponse>>();
            try
            {
                var pagination = await _unitOfWork.DailyOrderRepository.ToPagination(pageIndex, pageSize);
                result.Payload = _mapper.Map<Pagination<DailyOrderResponse>>(pagination);
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

        public async Task<OperationResult<DailyOrderResponse>> UpdateDailyOrder(Guid id, UpdateDailyOrderRequest updateDailyOrderRequest)
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
}
