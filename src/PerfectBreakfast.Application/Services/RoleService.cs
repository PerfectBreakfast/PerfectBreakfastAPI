using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                /*var roles = await _roleManager.Roles.ToListAsync();
                result.Payload = _mapper.Map<List<RoleResponse>>(roles);*/
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
                /*var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
                result.Payload = _mapper.Map<RoleResponse>(role);*/
            }
            /*catch (NotFoundIdException e)
            {
                result.AddError(ErrorCode.NotFound,e.Message);
            }*/
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
                // map model to Entity
                /*var role = _mapper.Map<Role>(requestModel);
                // Add to DB
                var entity = await _unitOfWork.RoleRepository.AddAsync(role);
                // save change 
                await _unitOfWork.SaveChangeAsync();
                // map model to response
                result.Payload = _mapper.Map<RoleResponse>(entity);*/
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
                /*var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
                // map from requestModel => supplier
                _mapper.Map(requestModel, role);
                // update
                _unitOfWork.RoleRepository.Update(role);
                // saveChange
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<RoleResponse>(role);*/
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
                /*var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
                // Remove
                var entity = _unitOfWork.RoleRepository.Remove(role);
                // saveChange
                await _unitOfWork.SaveChangeAsync();
                // map entity to SupplierResponse
                result.Payload = _mapper.Map<RoleResponse>(entity);*/
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        
    }
}
