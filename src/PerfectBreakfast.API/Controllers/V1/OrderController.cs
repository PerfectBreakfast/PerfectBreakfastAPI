using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.OrderModel.Request;
using PerfectBreakfast.Application.Utils;

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

        /// <summary>
        /// API for Customer
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Policy = ConstantRole.RequireCustomerRole)]
        public async Task<IActionResult> CreateOrder(OrderRequest orderRequest)
        {
            var response = await _orderService.CreateOrder(orderRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var response = await _orderService.GetOrders();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for all 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var response = await _orderService.GetOrder(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
        
        /// <summary>
        /// Api For Customer
        /// </summary>
        /// <returns></returns>s
        [HttpGet("history"), Authorize(policy:ConstantRole.RequireCustomerRole)]
        public async Task<IActionResult> GetOrderHistoryByCustomer()
        {
            var response = await _orderService.GetOrderHistory();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin, Customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateOrderRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "SUPER ADMIN, CUSTOMER")]
        public async Task<IActionResult> UpdateOrder(Guid id, UpdateOrderRequest updateOrderRequest)
        {
            var response = await _orderService.UpdateOrder(id, updateOrderRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("pagination"), Authorize(policy:ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> GetOrderPagination(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _orderService.GetOrderPaginationAsync(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Customer, Super Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SUPER ADMIN, CUSTOMER")]
        public async Task<IActionResult> RemoveOrder(Guid id)
        {
            var response = await _orderService.DeleteOrder(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// Api for Delivery Staff or Delivery Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/status-complete")]
        [Authorize]
        public async Task<IActionResult> CompleteOrder(Guid id)
        {
            var response = await _orderService.CompleteOrder(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
