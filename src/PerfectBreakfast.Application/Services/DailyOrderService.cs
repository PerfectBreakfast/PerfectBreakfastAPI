using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Request;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services
{
    public class DailyOrderService : IDailyOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DailyOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<DailyOrderResponse>> AutoUpdate(DateTime dateTime)
        {
            var result = new OperationResult<DailyOrderResponse>();
            try
            {
                var dailyOrders = await _unitOfWork.DailyOrderRepository.FindByCreationDate(dateTime);
                if (dailyOrders.Count > 0)
                {
                    foreach (var dailyOrderEntity in dailyOrders)
                    {
                        var orders = dailyOrderEntity.Orders;
                        var totalOrderPrice = orders.Sum(o => o.TotalPrice);
                        var totalOrder = orders.Count();
                        dailyOrderEntity.TotalPrice = totalOrderPrice;
                        dailyOrderEntity.OrderQuantity = totalOrder;
                        dailyOrderEntity.Status = DailyOrderStatus.Fulfilled;
                        _unitOfWork.DailyOrderRepository.Update(dailyOrderEntity);
                    }
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    result.AddUnknownError("Today doesn't have daily order");
                    return result;
                }
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
