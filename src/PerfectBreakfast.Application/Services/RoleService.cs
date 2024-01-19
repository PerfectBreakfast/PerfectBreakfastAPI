using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using Microsoft.AspNetCore.Identity;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<OperationResult<List<RoleResponse>>> GetAllRoles()
        {
            var result = new OperationResult<List<RoleResponse>>();
            try
            {
                var roles = await _unitOfWork.RoleRepository.GetAllAsync();
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
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
                result.Payload = _mapper.Map<RoleResponse>(role);
            }
            catch (NotFoundIdException e)
            {
                result.AddError(ErrorCode.NotFound,e.Message);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
        public async Task<OperationResult<bool>> CreateRole(CreatRoleRequest requestModel)
        {
            var result = new OperationResult<bool>();
            try
            {
                var role = new Role { Name = requestModel.Name };
                result.Payload = await _unitOfWork.RoleRepository.AddAsync(role);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<bool>> UpdateRole(Guid roleId, UpdateRolerequest requestModel)
        {
            var result = new OperationResult<bool>();
            try
            {
                // find supplier by ID
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
                _mapper.Map(requestModel, role);
                result.Payload = await _unitOfWork.RoleRepository.Update(role);
            }
            catch (NotFoundIdException e)
            {
                result.AddError(ErrorCode.NotFound,e.Message);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
        
        public async Task<OperationResult<bool>> RemoveRole(Guid roleId)
        {
            var result = new OperationResult<bool>();
            try
            {
                // find supplier by ID
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
                result.Payload = await _unitOfWork.RoleRepository.Delete(role);
            }
            catch (NotFoundIdException e)
            {
                result.AddError(ErrorCode.NotFound,e.Message);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        
    }
}
