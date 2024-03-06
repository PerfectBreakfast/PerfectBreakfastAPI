using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.RoleModels.Request;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/roles")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            this._roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRole()
        {
            var response = await _roleService.GetAllRoles();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleId(Guid id)
        {
            var response = await _roleService.GetRoleById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpGet("unit/{unitId}")]
        public async Task<IActionResult> GetRoleByUnitId(Guid unitId)
        {
            var response = await _roleService.GetRoleByUnitId(unitId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreatRoleRequest requestModel)
        {
            var response = await _roleService.CreateRole(requestModel);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, UpdateRolerequest requestModel)
        {
            var response = await _roleService.UpdateRole(id, requestModel);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveRole(Guid id)
        {
            var response = await _roleService.RemoveRole(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}
