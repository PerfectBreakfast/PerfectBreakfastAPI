using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Services;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/shippingorders")]
    public class ShippingOrderController : BaseController
    {
        private readonly IShippingOrderService _shippingOrderService;
        public ShippingOrderController(IShippingOrderService shippingOrderService)
        {
            _shippingOrderService = shippingOrderService;
        }

        [HttpPost("DeliveryAdmin"),Authorize(Policy = "RequireDeliveryAdminRole")]
        public async Task<IActionResult> CreateShippingOrder(CreateShippingOrderRequest requestModel)
        {
            var response = await _shippingOrderService.CreateShippingOrder(requestModel);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

    }
}
