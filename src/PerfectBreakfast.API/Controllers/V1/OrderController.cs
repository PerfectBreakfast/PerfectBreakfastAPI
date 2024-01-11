using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.OrderModel.Request;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/orders")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequest orderRequest)
        {
            var response = await _orderService.CreateOrder(orderRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
