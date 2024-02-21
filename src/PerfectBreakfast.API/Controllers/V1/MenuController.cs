using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MenuModels.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/menus")]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            var response = await _menuService.GetMenus();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> GetMenu(Guid id)
        {
            var response = await _menuService.GetMenu(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin
        /// </summary>
        /// <param name="createMenuFoodRequest"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> CreateMenu(CreateMenuFoodRequest createMenuFoodRequest)
        {
            var response = await _menuService.CreateMenu(createMenuFoodRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menuRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> UpdateMenu(Guid id, UpdateMenuRequest menuRequest)
        {
            var response = await _menuService.UpdateMenu(id, menuRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
        
        /// <summary>
        /// API for Super Admin
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("pagination"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> GetMenuPagination(string? searchTerm, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _menuService.GetMenuPaginationAsync(searchTerm, pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _menuService.DeleteMenu(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin, choose menu each date
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/menu-is-selected-status"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> ChooseMenu(Guid id)
        {
            var response = await _menuService.ChooseMenu(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin, menu is selected
        /// </summary>
        /// <returns></returns>
        [HttpGet("menu-is-selected"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> GetMenuToShow()
        {
            var response = await _menuService.GetMenuByStatus();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
