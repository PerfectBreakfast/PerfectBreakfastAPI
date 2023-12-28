using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var com = await _unitOfWork.RoleRepository.GetAllAsync();
                result.Payload = _mapper.Map<List<RoleResponse>>(com);
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
    }
}
