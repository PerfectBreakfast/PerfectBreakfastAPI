using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Request;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/daliyorders")]
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
        /// API For Supper Admin
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateDailyOrder(DailyOrderRequest dailyOrderRequest)
        {
            var response = await _dailyOrderService.CreateDailyOrder(dailyOrderRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetDailyOrderPagination(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _dailyOrderService.GetDailyOrderPaginationAsync(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API For Management Unit Admin
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDailyOrer(Guid id, UpdateDailyOrderRequest updateDailyOrderRequest)
        {
            var response = await _dailyOrderService.UpdateDailyOrder(id, updateDailyOrderRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API For Management Unit Admin
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("managementUnit")]
        public async Task<IActionResult> GetDailyOrderByManagementUnitId(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _dailyOrderService.GetDailyOrderByManagementUnit(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API For Management Unit Admin
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}/company")]
        public async Task<IActionResult> GetDailyOrderByManagementUnitId(Guid id, DateOnly bookingDate)
        {
            var response = await _dailyOrderService.GetDailyOrderDetailByManagementUnit(id, bookingDate);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

    }
}
