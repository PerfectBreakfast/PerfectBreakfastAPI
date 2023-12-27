using System.Linq.Expressions;
using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public Task<OperationResult<UserLoginResponse>> Login(LoginRequest query)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<List<UserResponse>>> GetAllUsers()
    {
        var result = new OperationResult<List<UserResponse>>();
        try
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<Pagination<UserResponse>>> GetUserPaginationAsync(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<UserResponse>>();
        try
        {
            var users = await _unitOfWork.UserRepository.ToPagination(pageIndex,pageSize);
            result.Payload = _mapper.Map<Pagination<UserResponse>>(users);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<UserResponse>> GetUserById(Guid id)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            result.Payload = _mapper.Map<UserResponse>(user);
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