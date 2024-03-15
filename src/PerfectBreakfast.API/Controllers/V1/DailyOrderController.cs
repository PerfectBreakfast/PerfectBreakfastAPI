using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DailyOrder.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/daily-orders")]
    public class DailyOrderController : BaseController
    {
        private readonly IDailyOrderService _dailyOrderService;

        public DailyOrderController(IDailyOrderService dailyOrderService)
        {
            _dailyOrderService = dailyOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyOrders()
        {
            var response = await _dailyOrderService.GetDailyOrders();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API For Super Admin
        /// </summary>
        /// <returns></returns>
        [HttpPost, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> CreateDailyOrder(DailyOrderRequest dailyOrderRequest)
        {
            var response = await _dailyOrderService.CreateDailyOrder(dailyOrderRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
        
        /// <summary>
        /// API for Super Admin
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("pagination"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> GetDailyOrderPagination(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _dailyOrderService.GetDailyOrderPaginationAsync(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API For Super Admin
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> UpdateDailyOrer(Guid id, UpdateDailyOrderRequest updateDailyOrderRequest)
        {
            var response = await _dailyOrderService.UpdateDailyOrder(id, updateDailyOrderRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API For Partner Admin
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("partner"), Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
        public async Task<IActionResult> GetDailyOrderByPartner(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _dailyOrderService.GetDailyOrderByPartner(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API For Delivery Admin
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("delivery"),Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
        public async Task<IActionResult> GetDailyOrderByDelivery(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _dailyOrderService.GetDailyOrderByDelivery(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
        
        /// <summary>
        /// API For Partner Admin
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("partner/initialization-completion"), Authorize(Policy = ConstantRole.RequirePartnerAdminRole)]
        public async Task<IActionResult> GetDailyOrderInitAndCompleteByPartner(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _dailyOrderService.GetDailyOrderInitAndCompleteByPartner(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
        
        /// <summary>
        /// API For Delivery Admin
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("delivery/initialization-completion"),Authorize(Policy = ConstantRole.RequireDeliveryAdminRole)]
        public async Task<IActionResult> GetDailyOrderInitAndCompleteByDelivery(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _dailyOrderService.GetDailyOrderInitAndCompleteByDelivery(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
