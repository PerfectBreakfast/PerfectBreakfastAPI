using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PerfectBreakfast.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper,RoleManager<IdentityRole<Guid>> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
        }
        public async Task<OperationResult<List<RoleResponse>>> GetAllRoles()
        {
            var result = new OperationResult<List<RoleResponse>>();
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                result.Payload = _mapper.Map<List<RoleResponse>>(roles);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
        public async Task<OperationResult<RoleResponse>> GetRoleById(Guid id)
        {
            var result = new OperationResult<RoleResponse>();
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role is null)
                {
                    result.AddError(ErrorCode.NotFound,$"Not found by Id: {id}");
                    return result;
                }
                result.Payload = _mapper.Map<RoleResponse>(role);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
        public async Task<OperationResult<RoleResponse>> CreateRole(CreatRoleRequest requestModel)
        {
            var result = new OperationResult<RoleResponse>();
            try
            {
                var role = new IdentityRole<Guid>(requestModel.Name);
                var entity = await _roleManager.CreateAsync(role);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<RoleResponse>> UpdateRole(Guid roleId, UpdateRolerequest requestModel)
        {
            var result = new OperationResult<RoleResponse>();
            try
            {
                // find supplier by ID
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role is null)
                {
                    result.AddError(ErrorCode.NotFound,$"Not found by Id: {roleId}");
                    return result;
                }
                _mapper.Map(requestModel, role);
                var isSuccess = await _roleManager.UpdateAsync(role);
                if (!isSuccess.Succeeded)
                {
                    result.AddError(ErrorCode.ServerError,$"Update not success");
                    return result;
                }
                result.Payload = _mapper.Map<RoleResponse>(role);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
        public async Task<OperationResult<RoleResponse>> RemoveRole(Guid roleId)
        {
            var result = new OperationResult<RoleResponse>();
            try
            {
                // find supplier by ID
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role is null)
                {
                    result.AddError(ErrorCode.NotFound,$"Not found by Id: {roleId}");
                    return result;
                }
                var isSuccess = await _roleManager.DeleteAsync(role);
                if (!isSuccess.Succeeded)
                {
                    result.AddError(ErrorCode.ServerError,$"Delete not success");
                    return result;
                }
                result.Payload = _mapper.Map<RoleResponse>(role);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        
    }
}
