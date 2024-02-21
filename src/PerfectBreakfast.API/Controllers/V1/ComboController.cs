using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ComboModels.Request;
using PerfectBreakfast.Application.Utils;

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

        /// <summary>
        /// API for Super Admin, Customer
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole), Authorize(Policy = ConstantRole.RequireCustomerRole)]
        public async Task<IActionResult> GetCombo(Guid id)
        {
            var response = await _comboService.GetCombo(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// AdPI for Super Admin
        /// </summary>
        /// <param name="createComboRequest"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> CreateCombo(CreateComboRequest createComboRequest)
        {
            var response = await _comboService.CreateCombo(createComboRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        /// <summary>
        /// API for Super Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comboRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}"), Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
        public async Task<IActionResult> UpdateCombo(Guid id, UpdateComboRequest comboRequest)
        {
            var response = await _comboService.UpdateCombo(id, comboRequest);
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
        public async Task<IActionResult> GetComboPagination(string? searchTerm, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _comboService.GetComboPaginationAsync(searchTerm, pageIndex, pageSize);
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
            var response = await _comboService.DeleteCombo(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
