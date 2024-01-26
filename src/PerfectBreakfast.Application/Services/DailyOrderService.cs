using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Request;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

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
                dailyOrder.Status = DailyOrderStatus.Pending;
                dailyOrder.OrderQuantity = 0;
                dailyOrder.TotalPrice = 0;
                await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<DailyOrderResponse>(dailyOrder);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<DailyOrderForManagemtUnitResponse>>> GetDailyOrderByManagementUnit()
        {
            var result = new OperationResult<List<DailyOrderForManagemtUnitResponse>>();
            var userId = _claimsService.GetCurrentUserId;
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                //// Xử lý dữ liệu để đẩy cho các đối tác theo cty
                var now = _currentTime.GetCurrentTime();
                var managementUnits = await _unitOfWork.ManagementUnitRepository.GetManagementUnits(now);
                var managementUnit = managementUnits.SingleOrDefault(m => m.Id == user.ManagementUnitId);

                if (managementUnit == null)
                {
                    result.AddUnknownError("managementUnit does not exsit");
                    return result;
                }

                // Lấy danh sách các công ty thuộc MU
                var companies = managementUnit.Companies;
                var dailyOrderForManagemtUnitResponses = new List<DailyOrderForManagemtUnitResponse>();

                // xử lý mỗi cty
                foreach (var company in companies)
                {
                    var dailyOrder = company.DailyOrders.SingleOrDefault(x => x.CreationDate.Date == now.Date);

                    DailyOrderForManagemtUnitResponse dailyOrderForManagemt = new DailyOrderForManagemtUnitResponse()
                    {
                        Id = company.Id,
                        Name = company.Name,
                        Address = company.Address,
                        StartWorkHour = company.StartWorkHour,
                        OrderQuantity = dailyOrder.OrderQuantity,
                        TotalPrice = dailyOrder.TotalPrice,
                        BookingDate = dailyOrder.BookingDate
                    };
                    dailyOrderForManagemtUnitResponses.Add(dailyOrderForManagemt);
                }
                result.Payload = dailyOrderForManagemtUnitResponses;

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

        public async Task<OperationResult<List<TotalFoodResponse>>> GetDailyOrderDetailByManagementUnit(Guid id)
        {
            var result = new OperationResult<List<TotalFoodResponse>>();
            try
            {
                var now = _currentTime.GetCurrentTime();
                var company = await _unitOfWork.CompanyRepository.GetCompanyById(id);
                if (company == null)
                {
                    result.AddUnknownError("Company is not exsit");
                    return result;
                }
                var foodCounts = new Dictionary<string, int>();

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
