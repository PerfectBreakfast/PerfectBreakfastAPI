using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Request;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/paymentMethods")]
    public class PaymentMethodController : BaseController
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var response = await _paymentMethodService.GetPaymentMethods();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentMethod(Guid id)
        {
            var response = await _paymentMethodService.GetPaymentMethod(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentMethod(PaymentMethodRequest paymentMethodRequest)
        {
            var response = await _paymentMethodService.CreatePaymentMethod(paymentMethodRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePaymentMethod(Guid id, PaymentMethodRequest paymentMethodRequest)
        {
            var response = await _paymentMethodService.UpdatePaymentMethod(id, paymentMethodRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> DeletePaymentMethod(Guid id)
        {
            var response = await _paymentMethodService.DeletePaymentMethod(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetPaymentMethodPagination(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _paymentMethodService.GetPaymentMethodPaginationAsync(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _paymentMethodService.Delete(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
