using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/combos")]
    public class ComboController : BaseController
    {
        private readonly IComboService _comboService;

        public ComboController(IComboService comboService)
        {
            _comboService = comboService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCombos()
        {
            var response = await _comboService.GetCombos();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCombo(Guid id)
        {
            var response = await _comboService.GetCombo(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCombo([FromForm] CreateComboRequest createComboRequest)
        {
            var response = await _comboService.CreateCombo(createComboRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCombo(Guid id, ComboRequest comboRequest)
        {
            var response = await _comboService.UpdateCombo(id, comboRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPut("{id}/combo-status")]
        public async Task<IActionResult> DeleteCombo(Guid id)
        {
            var response = await _comboService.DeleteCombo(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetComboPagination(int pageIndex = 0, int pageSize = 10)
        {
            var response = await _comboService.GetComboPaginationAsync(pageIndex, pageSize);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _comboService.Delete(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
