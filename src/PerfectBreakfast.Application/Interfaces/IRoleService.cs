using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IRoleService
    {
        public Task<OperationResult<List<RoleResponse>>> GetAllRoles();
        public Task<OperationResult<RoleResponse>> GetRoleById(Guid id);
        public Task<OperationResult<RoleResponse>> CreateRole(CreatRoleRequest requestModel);
        public Task<OperationResult<RoleResponse>> UpdateRole(Guid roleId, UpdateRolerequest requestModel);
        public Task<OperationResult<RoleResponse>> RemoveRole(Guid roleId);
    }
}
