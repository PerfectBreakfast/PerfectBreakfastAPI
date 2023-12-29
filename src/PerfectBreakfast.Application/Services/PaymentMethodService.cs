using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Request;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentMethodService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<PaymentMethodResponse>> CreatePaymentMethod(PaymentMethodRequest paymentMethodRequest)
        {
            var result = new OperationResult<PaymentMethodResponse>();
            try
            {
                var paymentMethod = _mapper.Map<PaymentMethod>(paymentMethodRequest);
                await _unitOfWork.PaymentMethodRepository.AddAsync(paymentMethod);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<PaymentMethodResponse>> Delete(Guid id)
        {
            var result = new OperationResult<PaymentMethodResponse>();
            try
            {
                var payment = await _unitOfWork.PaymentMethodRepository.GetByIdAsync(id);
                _unitOfWork.PaymentMethodRepository.Remove(payment);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<PaymentMethodResponse>> DeletePaymentMethod(Guid id)
        {
            var result = new OperationResult<PaymentMethodResponse>();
            try
            {
                var payment = await _unitOfWork.PaymentMethodRepository.GetByIdAsync(id);
                _unitOfWork.PaymentMethodRepository.SoftRemove(payment);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<PaymentMethodResponse>> GetPaymentMethod(Guid id)
        {
            var result = new OperationResult<PaymentMethodResponse>();
            try
            {
                var paymentMethod = await _unitOfWork.PaymentMethodRepository.GetByIdAsync(id);
                result.Payload = _mapper.Map<PaymentMethodResponse>(paymentMethod);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<Pagination<PaymentMethodResponse>>> GetPaymentMethodPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<PaymentMethodResponse>>();
            try
            {
                var payment = await _unitOfWork.PaymentMethodRepository.ToPagination(pageIndex, pageSize);
                result.Payload = _mapper.Map<Pagination<PaymentMethodResponse>>(payment);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<PaymentMethodResponse>>> GetPaymentMethods()
        {
            var result = new OperationResult<List<PaymentMethodResponse>>();
            try
            {
                var paymentMethods = await _unitOfWork.PaymentMethodRepository.GetAllAsync();
                result.Payload = _mapper.Map<List<PaymentMethodResponse>>(paymentMethods);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<PaymentMethodResponse>> UpdatePaymentMethod(Guid id, PaymentMethodRequest paymentMethodRequest)
        {
            var result = new OperationResult<PaymentMethodResponse>();
            try
            {
                var paymentMethod = _mapper.Map<PaymentMethod>(paymentMethodRequest);
                paymentMethod.Id = id;
                _unitOfWork.PaymentMethodRepository.Update(paymentMethod);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}
