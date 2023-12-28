using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IRoleService
    {
        public Task<OperationResult<List<RoleResponse>>> GetAllRoles();
        public Task<OperationResult<RoleResponse>> GetRoleById(Guid id);
    }
}
