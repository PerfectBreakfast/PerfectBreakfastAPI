using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MenuModels.Request;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/menus")]
    public class MenuComtroller : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuComtroller(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            var response = await _menuService.GetMenus();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu(Guid id)
        {
            var response = await _menuService.GetMenu(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu(CreateMenuFoodRequest createMenuFoodRequest)
        {
            var response = await _menuService.CreateMenu(createMenuFoodRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, MenuRequest menuRequest)
        {
            var response = await _menuService.UpdateMenu(id, menuRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            var response = await _menuService.DeleteMenu(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetMenuPagination(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _menuService.GetMenuPaginationAsync(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _menuService.Delete(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenuAndCombo(CreateMenuAndComboRequest createMenuAndComboRequest)
        {
            var response = await _menuService.CreateMenuAndCombo(createMenuAndComboRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
