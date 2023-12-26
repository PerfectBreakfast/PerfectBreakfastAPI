using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IUserService
{
    public Task<OperationResult<UserLoginResponse>> Login(LoginRequest query);
    public Task<OperationResult<List<UserResponse>>> GetAllUsers();
}